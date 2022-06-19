using VisionClient.Core.Dtos;
using VisionClient.Core.Enums;
using VisionClient.Core.Helpers;
using VisionClient.Core.Services.IServices;

namespace VisionClient.Core.Services
{
    public class CurrencyService : HttpService, ICurrencyService
    {
        public CurrencyService(IHttpClientFactory httpClientFactory, IStaticData staticData) : base(httpClientFactory, staticData)
        {
        }

        public async Task<ResponseDto> GetCurrencies()
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.GET,
                ApiUrl = $"{ConnectionData.GatewayUrl}/currency/getpackages"
            });

            if (response is not null) return response;
            return new ResponseDto();
        }

        public async Task<ResponseDto?> AddCurrencyPackage(AddCurrencyDto data)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.POST,
                ApiUrl = $"{ConnectionData.GatewayUrl}/currency/addpackage",
                Data = data
            });

            if (response is not null) return response;
            return null;
        }
    }
}
