using MessageService_API.Services.IServices;
using Microsoft.Extensions.Caching.Memory;

namespace MessageService_API.Services
{
    public class ConnectionsCacheService : IConnectionsCacheService
    {
        private readonly IMemoryCache memoryCache;
        public ConnectionsCacheService(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        public Task AddToGroupCache(Guid chatId, Guid userId, string connectionId)
        {
            Dictionary<Guid, List<string>> connectionIds;
            memoryCache.TryGetValue(chatId, out connectionIds);
            if (connectionIds is null) connectionIds = new();

            if (!connectionIds.Any(x => x.Key == userId))
            {
                connectionIds.Add(userId, new List<string>());
            }

            connectionIds[userId].Add(connectionId);
            memoryCache.Set(chatId, connectionIds);

            return Task.CompletedTask;
        }

        public Task<Dictionary<Guid, List<string>>> GetFromGroupCache(Guid chatId)
        {
            Dictionary<Guid, List<string>> connectionIds;
            memoryCache.TryGetValue(chatId, out connectionIds);
            if (connectionIds is null) connectionIds = new();

            return Task.FromResult(connectionIds);
        }

        public Task RemoveFromGroupCache(Guid chatId, Guid userId, string connectionId)
        {
            Dictionary<Guid, List<string>> connectionIds;
            memoryCache.TryGetValue(chatId, out connectionIds);
            if (connectionIds is null) connectionIds = new();

            if (connectionIds.Any(x => x.Key == userId))
            {
                connectionIds[userId].Remove(connectionId);
                if (connectionIds[userId].Count == 0) connectionIds.Remove(userId);

                if (connectionIds.Count == 0) memoryCache.Remove(chatId);
                else memoryCache.Set(chatId, connectionIds);
            }

            return Task.CompletedTask;
        }
    }
}