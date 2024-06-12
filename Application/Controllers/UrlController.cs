using System.Net;
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
    
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var allMappings = await _urlShortenerService.GetAll();
        return Ok(allMappings);
    }

    [HttpPost("shorten")]
    public async Task<IActionResult> ShortenUrl([FromBody] string longUrl)
    {
        var shortUrl = await _urlShortenerService.ShortenUrl(longUrl);
        return Ok(shortUrl);
    }

    [HttpGet("{shortUrl}")]
    public async Task<IActionResult> RedirectToLongUrl([FromRoute]string shortUrl)
    {
        var longUrl = await _urlShortenerService.GetLongUrl(shortUrl);

        if (longUrl == null)
            return NotFound();

        return Redirect(longUrl);
    }
}