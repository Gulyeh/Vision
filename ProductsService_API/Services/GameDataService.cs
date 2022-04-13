using ProdcutsService_API.Helpers;
using ProductsService_API.Helpers;
using ProductsService_API.Services.IServices;
using ProductsService_API.Statics;

namespace ProductsService_API.Services
{
    public class GameDataService : BaseHttpService, IGameDataService
    {
        public GameDataService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public async Task<T?> CheckGameExists<T>(Guid gameId, string Access_Token)
        {
            var response = await SendAsync<T>(new ApiRequest()
            {
                apiType = APIType.GET,
                ApiUrl = $"{ApiUrls.GamesData}api/games/checkgame?gameId={gameId}",
                Access_Token = Access_Token
            });

            if (response is not null) return response;
            return default(T);
        }
    }
}