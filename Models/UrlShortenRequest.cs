using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
namespace UrlShortener.Models
{
    public class UrlShortenRequest
    {
        [Required]
        [JsonPropertyName("originalUrl")]
        public string OriginalUrl { get; set; }
    }
}
