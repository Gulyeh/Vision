using ProdcutsService_API.Helpers;
using ProductsService_API.Dtos;
using ProductsService_API.Helpers;
using ProductsService_API.Services.IServices;

namespace ProductsService_API.Services
{
    public class GameAccessService : BaseHttpService, IGameAccessService
    {
        private readonly string GameAccessServiceUrl;

        public GameAccessService(IHttpClientFactory httpClientFactory, ILogger<BaseHttpService> logger, IConfiguration config) : base(httpClientFactory, logger)
        {
            GameAccessServiceUrl = config.GetValue<string>("GameAccessServiceUrl");
        }

        public async Task<ResponseDto?> CheckGameAccess(Guid gameId, string Access_Token)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                apiType = APIType.GET,
                ApiUrl = $"{GameAccessServiceUrl}/api/access/boughtgame?gameId={gameId}",
                Access_Token = Access_Token
            });

            if (response is not null) return response;
            return null;
        }

        public async Task<ResponseDto?> CheckProductAccess(Guid productId, Guid gameId, string Access_Token)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                apiType = APIType.GET,
                ApiUrl = $"{GameAccessServiceUrl}/api/access/OwnsProduct?productId={productId}&gameId={gameId}",
                Access_Token = Access_Token
            });

            if (response is not null) return response;
            return null;
        }
    }
}