using url_shortener_dotnet.Domain.Entities;

namespace url_shortener_dotnet.Domain.Interfaces;

public interface IUrlRepository
{
    UrlMapping GetByShortUrl(string shortUrl);
    void Save(UrlMapping urlMapping);
}