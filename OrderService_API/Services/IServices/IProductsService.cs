using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService_API.Dtos;
using OrderService_API.Helpers;

namespace OrderService_API.Services.IServices
{
    public interface IProductsService
    {
        Task<T?> CheckProductExists<T>(Guid gameId, string Access_Token, Guid? productId = null);
        Task<GameDto> GetGame(Guid gameId, string Access_Token);
    }
}