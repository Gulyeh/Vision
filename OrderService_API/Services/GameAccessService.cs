using OrderService_API.Helpers;
using OrderService_API.Services.IServices;

namespace OrderService_API.Services
{
    public class GameAccessService : BaseHttpService, IGameAccessService
    {
        private readonly string ProductServiceUrl;

        public GameAccessService(IHttpClientFactory httpClientFactory, ILogger<BaseHttpService> logger, IConfiguration config) : base(httpClientFactory, logger)
        {
            ProductServiceUrl = config.GetValue<string>("ProductServiceUrl");
        }

        public async Task<bool> CheckProductAccess(CreateOrderData data)
        {
            string url = data.OrderType switch
            {
                OrderType.Game => $"{ProductServiceUrl}api/access/CheckGameAccess?gameId={data.ProductId}",
                OrderType.Product => $"{ProductServiceUrl}api/products/OwnsProduct?productId={data.ProductId}&gameId={data.GameId}",
                _ => ""
            };

            var response = await SendAsync<HasProductAccess>(new ApiRequest()
            {
                apiType = APIType.GET,
                ApiUrl = url,
                Access_Token = data.Access_Token
            });

            if (response is not null) return response.HasAccess;
            return false;
        }
    }
}