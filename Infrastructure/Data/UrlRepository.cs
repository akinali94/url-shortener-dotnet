using url_shortener_dotnet.Domain.Entities;
using url_shortener_dotnet.Domain.Interfaces;

namespace url_shortener_dotnet.Infrastructure.Data;

public class UrlRepository : IUrlRepository
{
    private readonly Dictionary<string, UrlMapping> _urlMappings = new Dictionary<string, UrlMapping>();

    public UrlMapping GetByShortUrl(string shortUrl)
    {
        _urlMappings.TryGetValue(shortUrl, out var urlMapping);
        return urlMapping;
    }

    public void Save(UrlMapping urlMapping)
    {
        _urlMappings[urlMapping.ShortUrl] = urlMapping;
    }
}