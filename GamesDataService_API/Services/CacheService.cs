using GamesDataService_API.DbContexts;
using GamesDataService_API.Helpers;
using GamesDataService_API.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GamesDataService_API.Services
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

        public Task TryAddToCache<T>(CacheType type, T data) where T : class
        {
            List<T> value;
            if (memoryCache.TryGetValue(type, out value))
            {
                lock (value)
                {
                    value.Add(data);
                    SetCache<T>(type, value);
                }
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<T>> TryGetFromCache<T>(CacheType type) where T : class
        {
            IEnumerable<T> value;
            if (!memoryCache.TryGetValue(type, out value))
            {
                value = db.Set<T>();
                SetCache<T>(type, value);
            }
            return Task.FromResult(value);
        }

        public Task TryRemoveFromCache<T>(CacheType type, T data) where T : class
        {
            List<T> value;
            if (memoryCache.TryGetValue(type, out value))
            {
                lock (value)
                {
                    value.Remove(data);
                    SetCache<T>(type, value);
                }
            }
            return Task.CompletedTask;
        }

        private void SetCache<T>(CacheType type, IEnumerable<T> value)
        {
            var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            memoryCache.Set(type, value, cacheOptions);
        }
    }
}