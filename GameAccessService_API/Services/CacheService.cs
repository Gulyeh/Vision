using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GameAccessService_API.DbContexts;
using GameAccessService_API.Helpers;
using GameAccessService_API.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

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
            if(memoryCache.TryGetValue(type, out value)){
                lock(value){
                    value.Add(data);
                    SetCache<T>(type, value);
                }
            }
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<T>> TryGetFromCache<T>(CacheType type, Guid userId) where T : BaseUser
        {
            IEnumerable<T> value;
            if(!memoryCache.TryGetValue(type, out value)){
                value = await db.Set<T>().ToListAsync();
                SetCache<T>(type, value);
            }
            value = value.Where(x => x.UserId == userId);
            return value;
        }

        public Task TryRemoveFromCache<T>(CacheType type, T data) where T : BaseUser
        {
            List<T> value;
            if(memoryCache.TryGetValue(type, out value)){
                lock(value){
                    value.Remove(data);
                    SetCache<T>(type, value);
                }
            }
            return Task.CompletedTask;
        }

        private void SetCache<T>(CacheType type, IEnumerable<T> value){
            var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            memoryCache.Set(type, value, cacheOptions);
        }
    }
}