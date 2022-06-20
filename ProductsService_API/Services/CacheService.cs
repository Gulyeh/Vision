using Microsoft.Extensions.Caching.Memory;
using ProductsService_API.DbContexts;
using ProductsService_API.Helpers;
using ProductsService_API.Services.IServices;

namespace ProductsService_API.Services
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

        public Task TryAddToCache<T>(CacheType type, T data) where T : BaseProducts
        {
            List<T> value;
            memoryCache.TryGetValue(type, out value);
            if (value is null) value = new List<T>();
            value.Add(data);
            
            SetCache<T>(type, value);
            return Task.CompletedTask;
        }

        public Task<List<T>> TryGetFromCache<T>(CacheType type) where T : new()
        {
            List<T> value;
            memoryCache.TryGetValue(type, out value);
            if (value is null) return Task.FromResult(new List<T>());
            return Task.FromResult(value);
        }

        public Task DeleteFromCache<T>(CacheType type, T data) where T : BaseProducts
        {
            List<T> value;
            memoryCache.TryGetValue(type, out value);
            if (value is null) return Task.CompletedTask;

            value.Remove(data);
            SetCache<T>(type, value);

            return Task.CompletedTask;
        }

        private void SetCache<T>(CacheType type, IEnumerable<T> value) where T : BaseProducts
        {
            var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            memoryCache.Set(type, value, cacheOptions);
        }
    }
}