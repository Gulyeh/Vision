using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ProductsService_API.Helpers;

namespace ProductsService_API.Services.IServices
{
    public interface ICacheService
    {
        Task<IEnumerable<T>> TryGetFromCache<T>(CacheType type, Guid gameId) where T : class;
        Task TryRemoveFromCache<T>(CacheType type, Guid gameId) where T : class;
    }
}