using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageService_API.Services.IServices
{
    public interface IChatCacheService
    {
        Task RemoveFromChatCache(Guid user1Id, Guid user2Id);
        Task<Guid> GetFromChatCache(Guid user1Id, Guid user2Id);
        Task AddToChatCache(Guid chatId, Guid user1Id, Guid user2Id);
    }
}