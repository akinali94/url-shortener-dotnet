using url_shortener_dotnet.Domain.Entities;

namespace url_shortener_dotnet.Domain.Interfaces;

public interface IUrlRepository
{
    Task<UrlMapping> GetByShortUrl(string shortUrl);
    Task Save(UrlMapping urlMapping);
    Task<string> CheckLongUrl(string longUrl);
    Task<IEnumerable<UrlMapping>> GetAll();
}