using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Models
{
    public class UrlMapping
    {
        [Key]
        public string ShortUrl { get; set; }
        [Required]
        public string OriginalUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpirationTime { get; set; }
    }
}
