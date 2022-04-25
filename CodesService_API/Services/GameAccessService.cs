using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodesService_API.Helpers;
using CodesService_API.Services.IServices;
using CodesService_API.Statics;
using Newtonsoft.Json;

namespace CodesService_API.Services
{
    public class GameAccessService : BaseHttpService, IGameAccessService
    {
        public GameAccessService(IHttpClientFactory httpClientFactory, ILogger<BaseHttpService> logger) : base(httpClientFactory, logger)
        {
        }

        private class Response{
            public bool hasAccess { get; set; }
        }

        public async Task<bool> CheckAccess(string? gameId, string Access_Token, string? productId = null)
        {
           string url = string.Empty;
           if(productId is null) url = $"{APIUrls.GameAccessUrl}api/access/BoughtGame?gameId={gameId}";
           else url = $"{APIUrls.GameAccessUrl}api/access/BoughtProduct?gameId={gameId}&productId={productId}";

           var response = await SendAsync<Response>(new ApiRequest()
            {
                apiType = APIType.GET,
                ApiUrl = url,
                Access_Token = Access_Token
            });

            if(response is not null) return response.hasAccess;
            return false;
        }
    }
}