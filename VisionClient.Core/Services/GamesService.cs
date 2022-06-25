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

            return response;
        }

        public async Task<ResponseDto?> GetGames()
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.GET,
                ApiUrl = $"{ConnectionData.GatewayUrl}/Games/GetGames",
            });

            return response;
        }

        public async Task<ResponseDto?> GetNews(Guid gameId)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.GET,
                ApiUrl = $"{ConnectionData.GatewayUrl}/News/GetNews?gameId={gameId}",
            });

            return response;
        }

        public async Task<ResponseDto?> GetProducts(Guid gameId)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.GET,
                ApiUrl = $"{ConnectionData.GatewayUrl}/games/getproductgame?gameId={gameId}",
            });

            return response;
        }

        public async Task<ResponseDto?> CheckGameAccess(Guid gameId)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.GET,
                ApiUrl = $"{ConnectionData.GatewayUrl}/access/CheckGameAccess?gameId={gameId}",
            });

            return response;
        }

        public async Task<ResponseDto?> AddNews(AddNewsDto data)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.POST,
                ApiUrl = $"{ConnectionData.GatewayUrl}/news/AddNews",
                Data = data
            });

            return response;
        }

        public async Task<ResponseDto?> AddGamePackage(AddPackageDto data)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.POST,
                ApiUrl = $"{ConnectionData.GatewayUrl}/products/AddProduct",
                Data = data
            });

            return response;
        }

        public async Task<ResponseDto?> EditGame(EditGameDto data)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.PUT,
                ApiUrl = $"{ConnectionData.GatewayUrl}/games/EditGame",
                Data = data
            });

            return response;
        }

        public async Task<ResponseDto?> DeleteNews(Guid gameId, Guid newsId)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.DELETE,
                ApiUrl = $"{ConnectionData.GatewayUrl}/news/deletenews?newsId={newsId}&gameId={gameId}"
            });

            return response;
        }

        public async Task<ResponseDto?> EditNews(EditNewsDto data)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.PUT,
                ApiUrl = $"{ConnectionData.GatewayUrl}/news/editnews",
                Data = data
            });

            return response;
        }

        public async Task<ResponseDto?> DeleteGame(Guid gameId)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.DELETE,
                ApiUrl = $"{ConnectionData.GatewayUrl}/games/deletegame?gameId={gameId}"
            });

            return response;
        }

        public async Task<ResponseDto?> DeletePackage(Guid packageId, Guid gameId)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.DELETE,
                ApiUrl = $"{ConnectionData.GatewayUrl}/products/DeleteProduct?gameId={gameId}&productId={packageId}"
            });

            return response;
        }

        public async Task<ResponseDto?> EditPackage(EditPackageDto data)
        {
            string url = data.GameId == Guid.Empty ?
                $"{ConnectionData.GatewayUrl}/games/EditProductGame" :
                $"{ConnectionData.GatewayUrl}/products/EditProduct";

            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.PUT,
                ApiUrl = url,
                Data = data
            });

            return response;
        }
    }
}
