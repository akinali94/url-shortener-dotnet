using MongoDB.Driver;
using url_shortener_dotnet.Domain.Entities;
using url_shortener_dotnet.Domain.ExceptionHandling;
using url_shortener_dotnet.Domain.Interfaces;
using url_shortener_dotnet.Infrastructure.Configs;
using InvalidOperationException = url_shortener_dotnet.Domain.ExceptionHandling.InvalidOperationException;

namespace url_shortener_dotnet.Infrastructure.Data;

public class UrlRepository : IUrlRepository
{
    private readonly DbContext _database;

    public UrlRepository(DbContext database)
    {
        _database = database;
    }

    public async Task<UrlMapping> GetByShortUrl(string shortUrl)
    {
        var result  = await _database.UrlMappings
            .Find(x => x.ShortUrl == shortUrl).FirstOrDefaultAsync();

        if (result is null)
            throw new FetchingDataException("Short Url Not Found on Database");
        
        return result ;
    }

    public async Task Save(UrlMapping urlMapping)
    {
        try
        {
            Console.WriteLine($"Your new item's ID: {urlMapping.Id} \n long: {urlMapping.LongUrl} \n short: {urlMapping.ShortUrl}");
            await _database.UrlMappings.InsertOneAsync(urlMapping);
        }
        catch(Exception ex)
        {
            throw new InvalidOperationException($"Generate short url failed!: {ex.Message}");
        }
        
    }

    public async Task<string> CheckLongUrl(string longUrl)
    {
        Console.WriteLine($"Data check in repository");   
        var result = await _database.UrlMappings.Find(x => x.LongUrl == longUrl).FirstOrDefaultAsync();
        Console.WriteLine(result);

        if (result is null)
            return "null";
        
        return result.ShortUrl;

    }

    public async Task<IEnumerable<UrlMapping>> GetAll()
    {
        return await _database.UrlMappings.Find(x => true).ToListAsync();
    }
}