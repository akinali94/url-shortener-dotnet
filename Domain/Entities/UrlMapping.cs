using MongoDB.Bson.Serialization.Attributes;

namespace url_shortener_dotnet.Domain.Entities;

public class UrlMapping
{
    //Unique Id generator
    [BsonId]
    public string Id { get; set; }
    public string ShortUrl { get; set; }
    public string LongUrl { get; set; }
}