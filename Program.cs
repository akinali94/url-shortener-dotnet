using StackExchange.Redis;
using url_shortener_dotnet.Application.Middlewares;
using url_shortener_dotnet.Domain.Helpers;
using url_shortener_dotnet.Domain.Interfaces;
using url_shortener_dotnet.Domain.Services;
using url_shortener_dotnet.Infrastructure.Configs;
using url_shortener_dotnet.Infrastructure.Data;

namespace url_shortener_dotnet;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("UrlDatabase"));

        builder.Services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("REDIS_DB") ?? "localhost:6379"));
        
        builder.Services.AddScoped<DbContext>();
        builder.Services.AddScoped<IUrlRepository, UrlRepository>();
        builder.Services.AddScoped<IUrlShortenerService, UrlShortenerService>();
        
        builder.Services.AddScoped<Base58Encoder>();
        
        builder.Services.AddSingleton<SnowflakeIdGenerator>(provider =>
        {
            long datacenterId = long.Parse(Environment.GetEnvironmentVariable("DATACENTER_ID") ?? "1");
            long machineId = long.Parse(Environment.GetEnvironmentVariable("MACHINE_ID") ?? "1");

            return new SnowflakeIdGenerator(datacenterId, machineId);
        });

        // Add services to the container.
        builder.Services.AddAuthorization();

        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //app.UseHttpsRedirection();
        app.UseRouting();
        app.UseStaticFiles(); // Serve static files

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();

            // Serve the index.html file as the default page
            endpoints.MapGet("/", async context =>
            {
                context.Response.ContentType = "text/html";
                await context.Response.SendFileAsync(Path.Combine(env.WebRootPath, "index.html"));
            });
        });
        
        app.UseAuthorization();
        
        app.UseMiddleware<ExceptionMiddleware>();
        
        //Redis
        app.UseSlidingWindowRateLimiter();

        app.MapControllers();

        app.Run();
    }
}