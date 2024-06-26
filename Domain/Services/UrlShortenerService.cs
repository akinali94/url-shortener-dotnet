using url_shortener_dotnet.Domain.Entities;
using url_shortener_dotnet.Domain.Helpers;
using url_shortener_dotnet.Domain.Interfaces;

namespace url_shortener_dotnet.Domain.Services;

public class UrlShortenerService : IUrlShortenerService
{
    private readonly IUrlRepository _urlRepository;
    private readonly SnowflakeIdGenerator _snowflakeId;
    private readonly Base58Encoder _base58Encoder;

    public UrlShortenerService(IUrlRepository urlRepository,
                                SnowflakeIdGenerator snowflakeId,
                                Base58Encoder base58Encoder)
    {
        _urlRepository = urlRepository;
        _snowflakeId = snowflakeId;
        _base58Encoder = base58Encoder;
    }

    public async Task<string> ShortenUrl(string longUrl)
    {
        var checkResult = await _urlRepository.CheckLongUrl(longUrl);
        if (!string.IsNullOrEmpty(checkResult))
        {
            return checkResult;
        }
        
        var id = _snowflakeId.GenerateId();

        var shortUrl = _base58Encoder.Encode(id);
        
        var urlMapping = new UrlMapping
        {
            Id = id.ToString(),
            ShortUrl = shortUrl, 
            LongUrl = longUrl
        };
        
        await _urlRepository.Save(urlMapping);
        
        return shortUrl;
    }

    public async Task<string> GetLongUrl(string shortUrl)
    {
        var longUrl = await _urlRepository.GetByShortUrl(shortUrl);

        var result = longUrl.LongUrl;
        
        if (!result.StartsWith("http://") && !result.StartsWith("https://"))
        {
            result = "https://" + result;
        }

        return result;
        
    }

    public Task<IEnumerable<UrlMapping>> GetAll()
    {
        var getAll = _urlRepository.GetAll();
        
        return getAll;
    }
}