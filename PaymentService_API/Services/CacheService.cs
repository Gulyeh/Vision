using Microsoft.Extensions.Caching.Memory;
using PaymentService_API.Entities;
using PaymentService_API.Services.IServices;

namespace PaymentService_API.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache memoryCache;
        private const string cacheName = "paymentMethods";
        public CacheService(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        public Task AddMethodsToCache(PaymentMethods data)
        {
            List<PaymentMethods> value;
            memoryCache.TryGetValue(cacheName, out value);
            if (value is null) value = new List<PaymentMethods>();

            value.Add(data);
            SetCache(value);

            return Task.CompletedTask;
        }

        public Task<IEnumerable<PaymentMethods>> GetMethodsFromCache()
        {
            IEnumerable<PaymentMethods> value;
            memoryCache.TryGetValue(cacheName, out value);
            if (value is null) value = new List<PaymentMethods>();
            return Task.FromResult(value);
        }

        public Task RemoveMethodsFromCache(PaymentMethods data)
        {
            List<PaymentMethods> value;
            memoryCache.TryGetValue(cacheName, out value);
            if (value is null) return Task.CompletedTask;

            value.Remove(data);
            SetCache(value);

            return Task.CompletedTask;
        }

        private void SetCache(IEnumerable<PaymentMethods> value)
        {
            var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            memoryCache.Set(cacheName, value, cacheOptions);
        }
    }
}