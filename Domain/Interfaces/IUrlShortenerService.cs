using url_shortener_dotnet.Domain.Entities;

namespace url_shortener_dotnet.Domain.Interfaces;

public interface IUrlShortenerService
{
    Task<string> ShortenUrl(string longUrl);
    Task<string> GetLongUrl(string shortUrl);

    Task<IEnumerable<UrlMapping>> GetAll();
}