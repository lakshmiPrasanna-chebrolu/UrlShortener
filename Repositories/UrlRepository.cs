using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text;
using UrlShortener.Data;
using UrlShortener.Models;

namespace UrlShortener.Repositories
{
    public class UrlRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IDatabase _redisDb;
        public UrlRepository(ApplicationDbContext context, IConnectionMultiplexer redis)
        {
            _context = context;
            _redisDb = redis.GetDatabase();
        }
        public async Task<string> CreateShortUrl(string originalUrl)
        {
            var shortUrl = GenerateShortUrl(originalUrl);
            var urlMapping = new UrlMapping
            {
                ShortUrl = shortUrl,
                OriginalUrl = originalUrl
            };
            _context.UrlMappings.Add(urlMapping);
            await _context.SaveChangesAsync();
            await _redisDb.StringSetAsync(shortUrl, originalUrl, TimeSpan.FromDays(7));
            return shortUrl;
        }
        public async Task<string> GetOriginalUrl(string shortUrl)
        {
            var cachedUrl = await _redisDb.StringGetAsync(shortUrl);
            if (!string.IsNullOrEmpty(cachedUrl))
            {
                return cachedUrl;
            }
            var mapping = await _context.UrlMappings.FirstOrDefaultAsync(u => u.ShortUrl == shortUrl);
            if (mapping != null)
            {
                await _redisDb.StringSetAsync(shortUrl, mapping.OriginalUrl, TimeSpan.FromDays(7));
                return mapping.OriginalUrl;
            }
            return null;
        }
        private string GenerateShortUrl(string input)
        {
            int hash = input.GetHashCode();
            return Base62Encode(hash);
        }
        private string Base62Encode(int hash)
        {
            const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string result = "";
            hash = Math.Abs(hash);
            while (hash > 0)
            {
                result = chars[hash % 62] + result;
                hash /= 62;
            }
            return result.PadLeft(6, '0');
        }

    }
}
