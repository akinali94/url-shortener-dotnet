namespace url_shortener_dotnet.Domain.Entities;

public class UrlMapping
{
    //Unique Id generator
    public string Id { get; set; }
    public string ShortUrl { get; set; }
    public string LongUrl { get; set; }
}