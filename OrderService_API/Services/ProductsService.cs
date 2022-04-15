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

        public async Task<T?> CheckProductExists<T>(Guid productId, string Access_Token, OrderType orderType, Guid? gameId = null)
        {
            string url = orderType switch{
                OrderType.Currency => $"{APIUrls.ProductServiceUrl}api/currency/productexists?currencyId={productId}",
                OrderType.Game => $"{APIUrls.ProductServiceUrl}api/games/getgames?gameId={productId}",
                OrderType.Product => $"{APIUrls.ProductServiceUrl}api/products/GetProductsInGame?productId={productId}&gameId={gameId}",
                _ => ""
            };

            var response = await SendAsync<T>(new ApiRequest()
            {
                apiType = APIType.GET,
                ApiUrl = url,
                Access_Token = Access_Token
            });

            if (response is not null) return response;
            return default(T);
        }
    }
}