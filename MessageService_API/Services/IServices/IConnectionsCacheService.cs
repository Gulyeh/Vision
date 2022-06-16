namespace MessageService_API.Services.IServices
{
    public interface IConnectionsCacheService
    {
        Task RemoveFromGroupCache(Guid chatId, Guid userId, string connectionId);
        Task<Dictionary<Guid, List<string>>> GetFromGroupCache(Guid chatId);
        Task AddToGroupCache(Guid chatId, Guid userId, string connectionId);
    }
}