using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessageService_API.Services.IServices;
using Microsoft.Extensions.Caching.Memory;

namespace MessageService_API.Services
{
    public class ChatCacheService : IChatCacheService
    {
        private readonly IMemoryCache memoryCache;

        public ChatCacheService(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        public Task AddToChatCache(Guid chatId, Guid user1Id, Guid user2Id)
        {
            memoryCache.Set($"{user1Id}-{user2Id}", chatId);
            return Task.CompletedTask;
        }

        public Task<Guid> GetFromChatCache(Guid user1Id, Guid user2Id)
        {
            Guid chatId = Guid.Empty;
            memoryCache.TryGetValue($"{user1Id}-{user2Id}", out chatId);
            if(chatId == Guid.Empty) memoryCache.TryGetValue($"{user2Id}-{user1Id}", out chatId);
            return Task.FromResult(chatId);
        }

        public Task RemoveFromChatCache(Guid user1Id, Guid user2Id)
        {
            if(memoryCache.Get($"{user1Id}-{user2Id}") is not null){
                memoryCache.Remove($"{user1Id}-{user2Id}");
            }else if(memoryCache.Get($"{user2Id}-{user1Id}") is not null){
                memoryCache.Remove($"{user2Id}-{user1Id}");
            }

            return Task.CompletedTask;
        }
    }
}