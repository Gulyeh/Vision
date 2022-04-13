using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ProductsService_API.Entites;
using ProductsService_API.Helpers;

namespace ProductsService_API.Services.IServices
{
    public interface ICacheService
    {
        Task<T?> TryGetFromCache<T>(Guid gameId) where T : Games;
        Task TryAddToCache<T>(Guid gameId, T data) where T : Games;
    }
}