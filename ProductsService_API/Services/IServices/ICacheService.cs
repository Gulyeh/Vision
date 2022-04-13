using ProductsService_API.Entites;

namespace ProductsService_API.Services.IServices
{
    public interface ICacheService
    {
        Task<T?> TryGetFromCache<T>(Guid gameId) where T : Games;
        Task TryAddToCache<T>(Guid gameId, T data) where T : Games;
    }
}