using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GameAccessService_API.Helpers;

namespace GameAccessService_API.Services.IServices
{
    public interface ICacheService
    {
        Task<IEnumerable<T>> TryGetFromCache<T>(CacheType type) where T : class;
        Task TryRemoveFromCache<T>(CacheType type, T data) where T : class;
        Task TryAddToCache<T>(CacheType type, T data) where T : class;
    }
}