using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService_API.Services.IServices
{
    public interface ICacheService
    {
        Task<List<string>> TryGetFromCache(Guid userId);
        Task TryRemoveFromCache(Guid userId, string connId);
        Task TryAddToCache(Guid userId, string connId);
    }
}