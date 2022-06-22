using GameAccessService_API.DbContexts;
using GameAccessService_API.Helpers;
using GameAccessService_API.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Data;

namespace GameAccessService_API.Services
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

        public Task TryAddToCache<T>(CacheType type, T data) where T : BaseUser
        {
            List<T> value;
            memoryCache.TryGetValue(type, out value);
            if (value is null) value = new List<T>();

            value.Add(data);
            SetCache<T>(type, value);

            return Task.CompletedTask;
        }

        public Task<IEnumerable<T>> TryGetFromCache<T>(CacheType type, Guid userId) where T : BaseUser
        {
            IEnumerable<T> value;
            memoryCache.TryGetValue(type, out value);
            if (value is null)
            {
                value = db.Set<T>().ToList();
                SetCache<T>(type, value);
            }

            value = value.Where(x => x.UserId == userId).ToList();
            return Task.FromResult(value);
        }

        public Task TryRemoveFromCache<T>(CacheType type, T data) where T : BaseUser
        {
            List<T> value;
            if (memoryCache.TryGetValue(type, out value))
            {
                value.Remove(data);
                SetCache<T>(type, value);
            }
            return Task.CompletedTask;
        }

        private void SetCache<T>(CacheType type, IEnumerable<T> value)
        {
            var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            memoryCache.Set(type, value, cacheOptions);
        }

        public async Task TryUpdateCache<T>(CacheType type) where T : BaseUser
        {
            List<T> value = await db.Set<T>().ToListAsync();
            SetCache<T>(type, value);
        }
    }
}