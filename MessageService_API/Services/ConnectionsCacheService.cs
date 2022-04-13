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

        public Task AddToGroupCache(Guid chatId, string connectionId)
        {
            List<string> connectionIds = new List<string>();
            memoryCache.TryGetValue(chatId, out connectionIds);
            lock (connectionIds)
            {
                connectionIds.Add(connectionId);
                memoryCache.Set(chatId, connectionIds);
            }
            return Task.CompletedTask;
        }

        public Task<Dictionary<string, List<string>>> GetFromGroupCache(Guid chatId)
        {
            Dictionary<string, List<string>> connectionIds = new Dictionary<string, List<string>>();
            memoryCache.TryGetValue(chatId, out connectionIds);
            return Task.FromResult(connectionIds);
        }

        public Task RemoveFromGroupCache(Guid chatId, Guid userId, string connectionId)
        {
            Dictionary<string, List<string>> connectionIds = new Dictionary<string, List<string>>();
            memoryCache.TryGetValue(chatId, out connectionIds);
            lock (connectionIds)
            {
                if (connectionIds.ContainsKey(userId.ToString()))
                {
                    var userConnections = connectionIds[userId.ToString()];
                    userConnections.Remove(connectionId);
                    if (userConnections.Count == 0) connectionIds.Remove(userId.ToString());
                }
            }
            return Task.CompletedTask;
        }
    }
}