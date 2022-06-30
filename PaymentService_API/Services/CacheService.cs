using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PaymentService_API.DbContexts;
using PaymentService_API.Entities;
using PaymentService_API.Services.IServices;

namespace PaymentService_API.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache memoryCache;
        private readonly ApplicationDbContext db;
        private const string cacheName = "paymentMethods";
        public CacheService(IMemoryCache memoryCache, ApplicationDbContext db)
        {
            this.memoryCache = memoryCache;
            this.db = db;
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

        public async Task<IEnumerable<PaymentMethods>> GetMethodsFromCache()
        {
            IEnumerable<PaymentMethods> value;
            memoryCache.TryGetValue(cacheName, out value);
            if (value is null)
            {
                value = await db.PaymentMethods.ToListAsync();
            }
            return value;
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

        public Task ReplacePaymentMethod(PaymentMethods replacement)
        {
            List<PaymentMethods> value;
            memoryCache.TryGetValue(cacheName, out value);
            if (value is null) return Task.CompletedTask;

            var data = value.FirstOrDefault(x => x.Id == replacement.Id);
            if (data is not null)
            {
                value.Remove(data);
                value.Add(replacement);
                SetCache(value);
            }

            return Task.CompletedTask;
        }
    }
}