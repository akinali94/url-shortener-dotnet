using Microsoft.Extensions.Options;
using MongoDB.Driver;
using url_shortener_dotnet.Domain.Entities;

namespace url_shortener_dotnet.Infrastructure.Configs;

public class DbContext
{
    private readonly IMongoDatabase _database;
    private readonly string _collectionName;

    public DbContext(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
        _database = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _collectionName = mongoDbSettings.Value.CollectionName;
    }

    public IMongoCollection<UrlMapping> UrlMappings => _database.GetCollection<UrlMapping>(_collectionName);
}