using MongoDB.Driver;
using url_shortener_dotnet.Domain.Entities;
using url_shortener_dotnet.Domain.Interfaces;
using url_shortener_dotnet.Infrastructure.Configs;

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

        if (result == null)
            throw new Exception("Null from database");
        
        return result ;
    }

    public async Task Save(UrlMapping urlMapping)
    {
        await _database.UrlMappings.InsertOneAsync(urlMapping);
    }

    public async Task<IEnumerable<UrlMapping>> GetAll()
    {
        return await _database.UrlMappings.Find(x => true).ToListAsync();
    }
}