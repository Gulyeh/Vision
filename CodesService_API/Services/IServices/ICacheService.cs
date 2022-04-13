using CodesService_API.Helpers;

namespace CodesService_API.Services.IServices
{
    public interface ICacheService
    {
        Task<IEnumerable<T>> TryGetFromCache<T>(CacheType type) where T : class;
        Task TryRemoveFromCache<T>(CacheType type, T data) where T : class;
        Task TryAddToCache<T>(CacheType type, T data) where T : class;
    }
}