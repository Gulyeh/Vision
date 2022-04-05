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

        public async Task<IEnumerable<T>> TryGetFromCache<T>(CacheType type, Guid gameId) where T : class
        {
            IEnumerable<T> value;
            if(!memoryCache.TryGetValue($"{type}-{gameId}", out value)){
                value = await db.Set<T>().ToListAsync();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

                memoryCache.Set($"{type}-{gameId}", value, cacheOptions);
            }
            return value;
        }

        public Task TryRemoveFromCache<T>(CacheType type, Guid gameId) where T : class
        {
            IEnumerable<T> value;
            if(memoryCache.TryGetValue($"{type}-{gameId}", out value)){
                memoryCache.Remove($"{type}-{gameId}");
            }
            return Task.CompletedTask;
        }
    }
}