using Microsoft.AspNetCore.Mvc;
using url_shortener_dotnet.Domain.Interfaces;
using url_shortener_dotnet.Domain.Services;

namespace url_shortener_dotnet.Application.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UrlController : ControllerBase
{
    private readonly IUrlShortenerService _urlShortenerService;

    public UrlController(IUrlShortenerService urlShortenerService)
    {
        _urlShortenerService = urlShortenerService;
    }

    [HttpPost("shorten")]
    public IActionResult ShortenUrl([FromBody] string longUrl)
    {
        var shortUrl = _urlShortenerService.ShortenUrl(longUrl);
        return Ok(shortUrl);
    }

    [HttpGet("{shortUrl}")]
    public IActionResult RedirectToLongUrl([FromQuery]string shortUrl)
    {
        var longUrl = _urlShortenerService.GetLongUrl(shortUrl);
        
        if (longUrl == null)
        {
            return NotFound();
        }
        
        return Redirect(longUrl);
    }
}