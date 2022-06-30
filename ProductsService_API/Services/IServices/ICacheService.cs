using ProductsService_API.Entites;
using ProductsService_API.Helpers;

namespace ProductsService_API.Services.IServices
{
    public interface ICacheService
    {
        Task TryAddToCache<T>(CacheType type, T data) where T : BaseProducts;
        Task<List<T>> TryGetFromCache<T>(CacheType type) where T : new();
        Task DeleteFromCache<T>(CacheType type, T data) where T : BaseProducts;
        Task<List<Currency>> TryUpdateCurrency();
        Task TryReplaceCache<T>(CacheType type, T replacement) where T : BaseProducts;
    }
}