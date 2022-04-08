using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using UsersService_API.Helpers;
using UsersService_API.Services.IServices;

namespace UsersService_API.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        public Task<bool> TryAddToCache(OnlineUsersData data)
        {
            Dictionary<Guid, List<string>> value;
            if(memoryCache.TryGetValue(CacheType.OnlineUsers, out value)){
                lock(value){
                    if(value.ContainsKey(data.UserId)){
                        value[data.UserId].Add(data.connectionId);
                    }else{
                        value.Add(data.UserId, new List<string>{data.connectionId});
                    }
                    memoryCache.Set(CacheType.OnlineUsers, value);
                    return Task.FromResult(true);
                }
            }
            return Task.FromResult(false);
        }

        public Task<Dictionary<Guid, List<string>>> TryGetFromCache()
        {
            Dictionary<Guid, List<string>> value;
            memoryCache.TryGetValue(CacheType.OnlineUsers, out value);
            return Task.FromResult(value);
        }

        public Task<bool> TryRemoveFromCache(OnlineUsersData data)
        {
            bool isOnline = true;
            Dictionary<Guid, List<string>> value;
            if(memoryCache.TryGetValue(CacheType.OnlineUsers, out value)){
                lock(value){
                    if(value.ContainsKey(data.UserId)){
                        value[data.UserId].Remove(data.connectionId);
                        if(value[data.UserId].Count == 0){
                            value.Remove(data.UserId);
                            isOnline = false;
                        }
                        memoryCache.Set(CacheType.OnlineUsers, value);
                    }
                }
            }
            return Task.FromResult(isOnline);
        }
    }
}