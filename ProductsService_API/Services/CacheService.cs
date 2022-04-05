using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public Task TryAddToCache<T>(CacheType type, Guid gameId, T data) where T : class
        {
            List<T> value;
            string cacheName = $"{type}-{gameId}";
            if(memoryCache.TryGetValue(cacheName, out value)){
                value.Add(data);
                SetCache<T>(cacheName, value);
            }
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<T>> TryGetFromCache<T>(CacheType type, Guid gameId) where T : class
        {
            IEnumerable<T> value;
            string cacheName = $"{type}-{gameId}";
            if(!memoryCache.TryGetValue(cacheName, out value)){
                value = await db.Set<T>().ToListAsync();
                SetCache<T>(cacheName, value);
            }
            return value;
        }

        public Task TryRemoveFromCache<T>(CacheType type, Guid gameId, T data) where T : class
        {
            List<T> value;
            string cacheName = $"{type}-{gameId}";
            if(memoryCache.TryGetValue(cacheName, out value)){
                value.Remove(data);
                SetCache<T>(cacheName, value);
            }
            return Task.CompletedTask;
        }

        private void SetCache<T>(string type, IEnumerable<T> value){
            var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            memoryCache.Set(type, value, cacheOptions);
        }
    }
}