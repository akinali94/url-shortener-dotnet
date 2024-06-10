using url_shortener_dotnet.Domain.Entities;
using url_shortener_dotnet.Domain.Helpers;
using url_shortener_dotnet.Domain.Interfaces;

namespace url_shortener_dotnet.Domain.Services;

public class UrlShortenerService : IUrlShortenerService
{
    private readonly IUrlRepository _urlRepository;
    private readonly SnowflakeIdGenerator _snowflakeId;

    public UrlShortenerService(IUrlRepository urlRepository, SnowflakeIdGenerator snowflakeId)
    {
        _urlRepository = urlRepository;
        _snowflakeId = snowflakeId;
    }

    public string ShortenUrl(string longUrl)
    {
        var shortUrl = GenerateShortUrl();
        var urlMapping = new UrlMapping
        {
            ShortUrl = shortUrl, 
            LongUrl = longUrl
        };
        _urlRepository.Save(urlMapping);
        return shortUrl;
    }

    public string GetLongUrl(string shortUrl)
    {
        var urlMapping = _urlRepository.GetByShortUrl(shortUrl);
        return urlMapping?.LongUrl;
    }

    private string GenerateShortUrl()
    {
        // Implement URL shortening logic (e.g., hash the long URL)
        return Guid.NewGuid().ToString().Substring(0, 8);
    }
}