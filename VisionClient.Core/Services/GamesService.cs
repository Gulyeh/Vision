using VisionClient.Core.Dtos;
using VisionClient.Core.Enums;
using VisionClient.Core.Helpers;
using VisionClient.Core.Services.IServices;

namespace VisionClient.Core.Services
{
    public class GamesService : HttpService, IGamesService
    {
        public GamesService(IHttpClientFactory httpClientFactory, IStaticData staticData) : base(httpClientFactory, staticData)
        {
        }

        public async Task<ResponseDto?> AddGame(AddGameDto data)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.POST,
                ApiUrl = $"{ConnectionData.GatewayUrl}/Games/AddGame",
                Data = data
            });

            if (response is null) return null;
            return response;
        }

        public async Task<ResponseDto?> BoughtPackage(Guid productId, Guid gameId)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.GET,
                ApiUrl = $"{ConnectionData.GatewayUrl}/Access/OwnsProduct?productId={productId}&gameId={gameId}",
            });

            if (response is null) return null;
            return response;
        }

        public async Task<ResponseDto?> GetGames()
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.GET,
                ApiUrl = $"{ConnectionData.GatewayUrl}/Games/GetGames",
            });

            if (response is null) return null;
            return response;
        }

        public async Task<ResponseDto?> GetNews(Guid gameId)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.GET,
                ApiUrl = $"{ConnectionData.GatewayUrl}/News/GetNews?gameId={gameId}",
            });

            if (response is not null) return response;
            return null;
        }

        public async Task<ResponseDto?> GetProducts(Guid gameId)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.GET,
                ApiUrl = $"{ConnectionData.GatewayUrl}/games/getproductgame?gameId={gameId}",
            });

            if (response is not null) return response;
            return null;
        }

        public async Task<ResponseDto?> CheckGameAccess(Guid gameId)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.GET,
                ApiUrl = $"{ConnectionData.GatewayUrl}/access/CheckGameAccess?gameId={gameId}",
            });

            if (response is not null) return response;
            return null;
        }

        public async Task<ResponseDto?> AddNews(AddNewsDto data)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.POST,
                ApiUrl = $"{ConnectionData.GatewayUrl}/news/AddNews",
                Data = data
            });

            if (response is not null) return response;
            return null;
        }

        public async Task<ResponseDto?> AddGamePackage(AddPackageDto data)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.POST,
                ApiUrl = $"{ConnectionData.GatewayUrl}/products/AddProduct",
                Data = data
            });

            if (response is not null) return response;
            return null;
        }
    }
}
