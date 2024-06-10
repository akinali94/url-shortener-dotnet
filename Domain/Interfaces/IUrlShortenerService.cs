namespace url_shortener_dotnet.Domain.Interfaces;

public interface IUrlShortenerService
{
    string ShortenUrl(string longUrl);
    string GetLongUrl(string shortUrl);

}