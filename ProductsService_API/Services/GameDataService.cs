using ProdcutsService_API.Helpers;
using ProductsService_API.Dtos;
using ProductsService_API.Helpers;
using ProductsService_API.Services.IServices;

namespace ProductsService_API.Services
{
    public class GameDataService : BaseHttpService, IGameDataService
    {
        private readonly string GamesDataServiceUrl;

        public GameDataService(IHttpClientFactory httpClientFactory, ILogger<BaseHttpService> logger, IConfiguration config) : base(httpClientFactory, logger)
        {
            GamesDataServiceUrl = config.GetValue<string>("GamesDataServiceUrl");
        }

        public async Task<ResponseDto> CheckGameExists(Guid gameId, string Access_Token)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                apiType = APIType.GET,
                ApiUrl = $"{GamesDataServiceUrl}/api/games/checkgame?gameId={gameId}",
                Access_Token = Access_Token
            });

            if (response is not null) return response;
            return new ResponseDto(false, StatusCodes.Status400BadRequest, false);
        }
    }
}