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

            return response;
        }

        public async Task<ResponseDto?> DeletePaymentMethod(Guid paymentMethodId)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.DELETE,
                ApiUrl = $"{ConnectionData.GatewayUrl}/Payment/DeletePaymentMethod?paymentId={paymentMethodId}"
            });

            return response;
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

        public async Task<ResponseDto?> GetUserPayments()
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.GET,
                ApiUrl = $"{ConnectionData.GatewayUrl}/Payment/GetUserPayments"
            });

            return response;
        }

        public async Task<ResponseDto?> UpdatePaymentMethod(EditPaymentDto data)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.PUT,
                ApiUrl = $"{ConnectionData.GatewayUrl}/Payment/UpdatePaymentMethod",
                Data = data
            });

            return response;
        }
    }
}
