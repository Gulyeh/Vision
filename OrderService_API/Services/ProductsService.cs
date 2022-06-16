using OrderService_API.Helpers;
using OrderService_API.Services.IServices;

namespace OrderService_API.Services
{
    public class ProductsService : BaseHttpService, IProductsService
    {
        private readonly string ProductServiceUrl;

        public ProductsService(IHttpClientFactory httpClientFactory, ILogger<BaseHttpService> logger, IConfiguration config) : base(httpClientFactory, logger)
        {
            ProductServiceUrl = config.GetValue<string>("ProductServiceUrl");
        }

        public async Task<T?> CheckProductExists<T>(Guid productId, string Access_Token, OrderType orderType, Guid? gameId = null)
        {
            string url = orderType switch
            {
                OrderType.Currency => $"{ProductServiceUrl}api/currency/getpackages",
                OrderType.Game => $"{ProductServiceUrl}api/games/GetProductGame?gameId={productId}",
                OrderType.Product => $"{ProductServiceUrl}api/products/GetGameProducts?productId={productId}&gameId={gameId}",
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