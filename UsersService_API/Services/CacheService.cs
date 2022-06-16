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
            try
            {
                Dictionary<Guid, List<string>> value;
                memoryCache.TryGetValue(data.HubType, out value);
                if (value is null) value = new Dictionary<Guid, List<string>>();

                if (value.ContainsKey(data.UserId))
                {
                    value[data.UserId].Add(data.connectionId);
                }
                else
                {
                    value.Add(data.UserId, new List<string>() { data.connectionId });
                }
                memoryCache.Set(data.HubType, value);
                return Task.FromResult(true);

            }
            catch (Exception) { return Task.FromResult(false); }
        }

        public Task<Dictionary<Guid, List<string>>> TryGetFromCache(HubTypes hubType)
        {
            Dictionary<Guid, List<string>> value;
            memoryCache.TryGetValue(hubType, out value);
            if (value is null) value = new Dictionary<Guid, List<string>>();
            return Task.FromResult(value);
        }

        public Task<bool> TryRemoveFromCache(OnlineUsersData data)
        {
            bool isOnline = true;
            Dictionary<Guid, List<string>> value;
            if (memoryCache.TryGetValue(data.HubType, out value))
            {
                if (value.ContainsKey(data.UserId))
                {
                    value[data.UserId].Remove(data.connectionId);
                    if (value[data.UserId].Count == 0)
                    {
                        value.Remove(data.UserId);
                        isOnline = false;
                    }
                    memoryCache.Set(data.HubType, value);
                }
            }
            return Task.FromResult(isOnline);
        }
    }
}