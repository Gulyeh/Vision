using Microsoft.Extensions.Caching.Memory;
using OrderService_API.DbContexts;
using OrderService_API.Services.IServices;

namespace OrderService_API.Services
{
    public class CacheService : ICacheService
    {
        private readonly ApplicationDbContext db;
        private readonly IMemoryCache memoryCache;

        public CacheService(ApplicationDbContext db, IMemoryCache memoryCache)
        {
            this.db = db;
            this.memoryCache = memoryCache;
        }

        public Task TryAddToCache(Guid userId, string connId)
        {
            List<string> value;
            memoryCache.TryGetValue(userId, out value);
            if (value is null) value = new();

            value.Add(connId);
            SetCache(userId, value);

            return Task.CompletedTask;
        }

        public Task<List<string>> TryGetFromCache(Guid userId)
        {
            List<string> value;
            memoryCache.TryGetValue(userId, out value);
            if (value is null) value = new();

            return Task.FromResult(value);
        }

        public Task TryRemoveFromCache(Guid userId, string connId)
        {
            List<string> value;
            memoryCache.TryGetValue(userId, out value);

            if (value.Count > 0)
            {
                value.Remove(connId);
                if (value.Count() == 0) memoryCache.Remove(userId);
                else SetCache(userId, value);
            }

            return Task.CompletedTask;
        }

        private void SetCache(Guid userId, List<string> value)
        {
            var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            memoryCache.Set(userId, value, cacheOptions);
        }
    }
}