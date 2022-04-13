namespace OrderService_API.Services.IServices
{
    public interface ICacheService
    {
        Task<List<string>> TryGetFromCache(Guid userId);
        Task TryRemoveFromCache(Guid userId, string connId);
        Task TryAddToCache(Guid userId, string connId);
    }
}