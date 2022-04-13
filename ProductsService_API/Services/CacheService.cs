using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProductsService_API.DbContexts;
using ProductsService_API.Entites;
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

        public Task TryAddToCache<T>(Guid gameId, T data) where T : Games
        {
            T value;
            string cacheName = gameId.ToString();
            if(memoryCache.TryGetValue(cacheName, out value)){
                lock(value){
                    SetCache<T>(cacheName, data);
                }
            }
            return Task.CompletedTask;
        }

        public async Task<T?> TryGetFromCache<T>(Guid gameId) where T : Games
        {
            T? value;
            string cacheName = gameId.ToString();
            if(!memoryCache.TryGetValue(cacheName, out value)){
                value = await db.Set<T>().Include(x => x.GameProducts).FirstOrDefaultAsync(x => x.GameId == gameId);
                if(value is not null) SetCache<T>(cacheName, value);
            }
            return value;
        }

        private void SetCache<T>(string type, T value){
            var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            memoryCache.Set(type, value, cacheOptions);
        }
    }
}