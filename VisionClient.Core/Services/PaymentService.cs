using VisionClient.Core.Dtos;
using VisionClient.Core.Enums;
using VisionClient.Core.Helpers;
using VisionClient.Core.Services.IServices;

namespace VisionClient.Core.Services
{
    public class PaymentService : HttpService, IPaymentService
    {
        public PaymentService(IHttpClientFactory httpClientFactory, IStaticData staticData) : base(httpClientFactory, staticData)
        {
        }

        public async Task<ResponseDto?> CreatePaymentMethod(AddPaymentMethodDto data)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.POST,
                ApiUrl = $"{ConnectionData.GatewayUrl}/Payment/AddPaymentMethod",
                Data = data
            });

            if (response is not null) return response;
            return null;
        }

        public async Task<ResponseDto> GetNewProviders()
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.GET,
                ApiUrl = $"{ConnectionData.GatewayUrl}/Payment/GetNewProviders",
            });

            if (response is not null) return response;
            return new ResponseDto();
        }

        public async Task<ResponseDto> GetPaymentMethods()
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.GET,
                ApiUrl = $"{ConnectionData.GatewayUrl}/Payment/GetPaymentMethods",
            });

            if (response is not null) return response;
            return new ResponseDto();
        }
    }
}
