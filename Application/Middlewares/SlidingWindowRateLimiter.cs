using System.Net.Http.Headers;
using System.Text;
using StackExchange.Redis;
using url_shortener_dotnet.Domain.Helpers;
using url_shortener_dotnet.Infrastructure.Configs;

namespace url_shortener_dotnet.Application.Middlewares;

public static class SlidingWindowRateLimiterExtensions
{
    public static void UseSlidingWindowRateLimiter(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<SlidingWindowRateLimiter>();
    }
}

public class SlidingWindowRateLimiter
{
    //Lua Script
    private const string SlidingRateLimiter = @"
    local current_time = redis.call('TIME')
    local num_windows = ARGV[1]
    for i=2, num_windows*2, 2 do
        local window = ARGV[i]
        local max_requests = ARGV[i+1]
        local curr_key = KEYS[i/2]
        local trim_time = tonumber(current_time[1]) - window
        redis.call('ZREMRANGEBYSCORE', curr_key, 0, trim_time)
        local request_count = redis.call('ZCARD',curr_key)
        if request_count >= tonumber(max_requests) then
            return 1
        end
    end
    for i=2, num_windows*2, 2 do
        local curr_key = KEYS[i/2]
        local window = ARGV[i]
        redis.call('ZADD', curr_key, current_time[1], current_time[1] .. current_time[2])
        redis.call('EXPIRE', curr_key, window)
    end
    return 0
    ";
    
    private readonly IDatabase _redisDatabase;
    private readonly IConfiguration _config;
    private readonly RequestDelegate _next;

    public SlidingWindowRateLimiter(RequestDelegate next, IConnectionMultiplexer muxer, IConfiguration config)
    {
        _redisDatabase = muxer.GetDatabase();
        _config = config;
        _next = next;
    }

    //Get Api Key as IP Adress
    private static string GetApiKey(HttpContext context)
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        if (string.IsNullOrEmpty(ipAddress))
            return string.Empty;

        // Encode the IP address to ensure it is URL-safe
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(ipAddress));
        
    }

    //Redis tutorial implements BasicAuth for Rate Limiting
    // private static string GetApiKey(HttpContext context)
    // {
    //     var encoded = string.Empty;
    //     var auth = context.Request.Headers["Authorization"];
    //     if (!string.IsNullOrEmpty(auth)) 
    //         encoded = AuthenticationHeaderValue.Parse(auth).Parameter;
    //     if (string.IsNullOrEmpty(encoded)) 
    //         return encoded;
    //     return Encoding.UTF8.GetString(Convert.FromBase64String(encoded)).Split(':')[0];
    // }
    
    public IEnumerable<RateLimitRule> GetApplicableRules(HttpContext context)
    {
        var limits = _config.GetSection("RedisRateLimits").Get<RateLimitRule[]>();
        var applicableRules = limits
            .Where(x => x.MatchPath(context.Request.Path))
            .OrderBy(x => x.MaxRequests)
            .GroupBy(x => new{x.PathKey, x.WindowSeconds})
            .Select(x=>x.First());
        return applicableRules;
    }
    
    private async Task<bool> IsLimited( IEnumerable<RateLimitRule> rules, string apiKey)
    {
        var keys = rules.Select(x => new RedisKey($"{x.PathKey}:{{{apiKey}}}:{x.WindowSeconds}")).ToArray();
        var args = new List<RedisValue>{rules.Count()};
        foreach (var rule in rules)
        {
            args.Add(rule.WindowSeconds);
            args.Add(rule.MaxRequests);
        }
        return (int) await _redisDatabase.ScriptEvaluateAsync(SlidingRateLimiter, keys,args.ToArray()) == 1;
    }
    
    public async Task InvokeAsync(HttpContext httpContext)
    {
        var apiKey = GetApiKey(httpContext);
        if (string.IsNullOrEmpty(apiKey))
        {
            httpContext.Response.StatusCode = 401;
            return;
        }
        var applicableRules = GetApplicableRules(httpContext);
        var limited = await IsLimited(applicableRules, apiKey);
        if (limited)
        {
            httpContext.Response.StatusCode = 429;
            return;
        }
        await _next(httpContext);
    }

}