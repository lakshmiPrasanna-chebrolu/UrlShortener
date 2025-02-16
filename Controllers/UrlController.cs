using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Models;
using UrlShortener.Repositories;

namespace UrlShortener.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlController : ControllerBase
    {
        private readonly UrlRepository _urlRepository;
        public UrlController(UrlRepository urlRepository)
        {
            _urlRepository = urlRepository;
        }
        [HttpPost("shorten")]
        public async Task<IActionResult> ShortenUrl([FromBody] UrlShortenRequest request)
        {
            if (string.IsNullOrEmpty(request.OriginalUrl))
            {
                return BadRequest(new { message="The originalUrl field is required." });
            }
            var shortUrl = await _urlRepository.CreateShortUrl(request.OriginalUrl);
            return Ok(new { shortUrl = $"https://short.ly/{shortUrl}" });
        }
        [HttpGet("{shortUrl}")]
        public async Task<IActionResult> RedirectToOriginalUrl(string shortUrl)
        {
            var originalUrl=await _urlRepository.GetOriginalUrl(shortUrl);
            if(originalUrl == null) return NotFound("URL not Found");
            return Redirect(originalUrl);
        }

    }
}
