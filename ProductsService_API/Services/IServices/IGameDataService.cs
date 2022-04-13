using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsService_API.Services.IServices
{
    public interface IGameDataService
    {
        Task<T?> CheckGameExists<T>(Guid gameId, string Access_Token);
    }
}