using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersService_API.Helpers;

namespace UsersService_API.Services.IServices
{
    public interface ICacheService
    {
        Task<bool> TryAddToCache(OnlineUsersData data);
        Task<bool> TryRemoveFromCache(OnlineUsersData data);
        Task<Dictionary<Guid, List<string>>> TryGetFromCache();
    }
}