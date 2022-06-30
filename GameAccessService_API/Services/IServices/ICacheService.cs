using GameAccessService_API.Helpers;

namespace GameAccessService_API.Services.IServices
{
    public interface ICacheService
    {
        Task<IEnumerable<T>> TryGetFromCache<T>(CacheType type, Guid userId) where T : BaseUser;
        Task TryRemoveFromCache<T>(CacheType type, T data) where T : BaseUser;
        Task TryAddToCache<T>(CacheType type, T data) where T : BaseUser;
        Task TryUpdateCache<T>(CacheType type) where T : BaseUser;
    }
}