using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService_API.Dtos;
using OrderService_API.Helpers;
using OrderService_API.Services.IServices;
using OrderService_API.Statics;

namespace OrderService_API.Services
{
    public class ProductsService : BaseHttpService, IProductsService
    {
        public ProductsService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public async Task<T?> CheckProductExists<T>(Guid gameId, string Access_Token, Guid? productId = null)
        {
            var response = await SendAsync<T>(new ApiRequest(){
                apiType = APIType.GET,
                ApiUrl = $"{APIUrls.ProductServiceUrl}api/games/productexists?gameId={gameId}&productId={productId}",
                Access_Token = Access_Token
            });

            if(response is not null) return response;
            return default(T);
        }

        public async Task<GameDto> GetGame(Guid gameId, string Access_Token)
        {
            var response = await SendAsync<GameDto>(new ApiRequest(){
                apiType = APIType.GET,
                ApiUrl = $"{APIUrls.ProductServiceUrl}api/games/GetGame?gameId={gameId}",
                Access_Token = Access_Token
            });

            if(response is not null) return response;
            return new GameDto();
        }
    }
}