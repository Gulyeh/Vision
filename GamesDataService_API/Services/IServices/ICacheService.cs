using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GamesDataService_API.Helpers;

namespace GamesDataService_API.Services.IServices
{
    public interface ICacheService
    {
        Task<IEnumerable<T>> TryGetFromCache<T>(CacheType type) where T : class;
        Task TryRemoveFromCache<T>(CacheType type) where T : class;
    }
}