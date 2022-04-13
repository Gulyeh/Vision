using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageService_API.Services.IServices
{
    public interface IConnectionsCacheService
    {
        Task RemoveFromGroupCache(Guid chatId, Guid userId, string connectionId);
        Task<Dictionary<string, List<string>>> GetFromGroupCache(Guid chatId);
        Task AddToGroupCache(Guid chatId, string connectionId);
    }
}