using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CodesService_API.DbContexts;
using CodesService_API.Helpers;
using CodesService_API.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CodesService_API.Services
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

        public async Task<IEnumerable<T>> TryGetFromCache<T>(CacheType type) where T : class
        {
            IEnumerable<T> value;
            if(!memoryCache.TryGetValue(type, out value)){
                value = await db.Set<T>().ToListAsync();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

                memoryCache.Set(type, value, cacheOptions);
            }
            return value;
        }

        public Task TryRemoveFromCache<T>(CacheType type) where T : class
        {
            IEnumerable<T> value;
            if(memoryCache.TryGetValue(type, out value)){
                memoryCache.Remove(type);
            }
            return Task.CompletedTask;
        }
    }
}