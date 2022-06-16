using CodesService_API.Helpers;
using CodesService_API.Services.IServices;

namespace CodesService_API.Services
{
    public class GameAccessService : BaseHttpService, IGameAccessService
    {
        public readonly string AccessServiceUrl;
        public GameAccessService(IHttpClientFactory httpClientFactory, ILogger<BaseHttpService> logger, IConfiguration config) : base(httpClientFactory, logger)
        {
            AccessServiceUrl = config.GetValue<string>("AccessServiceUrl");
        }

        private class Response
        {
            public bool HasAccess { get; set; }
        }

        public async Task<bool> CheckAccess(Guid gameId, string Access_Token, Guid productId)
        {
            string url = string.Empty;
            if (gameId == Guid.Empty) url = $"{AccessServiceUrl}/api/access/BoughtGame?gameId={productId}";
            else url = $"{AccessServiceUrl}/api/access/OwnsProduct?productId={productId}&gameId={gameId}";

            var response = await SendAsync<Response>(new ApiRequest()
            {
                apiType = APIType.GET,
                ApiUrl = url,
                Access_Token = Access_Token
            });

            if (response is not null) return response.HasAccess;
            return false;
        }
    }
}