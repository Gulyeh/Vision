using VisionClient.Core.Dtos;
using VisionClient.Core.Enums;
using VisionClient.Core.Helpers;
using VisionClient.Core.Services.IServices;

namespace VisionClient.Core.Services
{
    public class OrderService : HttpService, IOrderService
    {
        public OrderService(IHttpClientFactory httpClientFactory, IStaticData staticData) : base(httpClientFactory, staticData)
        {
        }

        public async Task<ResponseDto?> ChangeOrderToPaid(Guid orderId)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.POST,
                ApiUrl = $"{ConnectionData.GatewayUrl}/order/ChangeToPaid?orderId={orderId}"
            });

            return response;
        }

        public async Task<ResponseDto?> GetOrders(string? orderId = null)
        {
            var url = $"{ConnectionData.GatewayUrl}/order/GetUserOrders";
            if (orderId is not null) url = $"{ConnectionData.GatewayUrl}/order/GetOrders?orderId={orderId}";

            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.GET,
                ApiUrl = url
            });

            return response;
        }

        public async Task<ResponseDto?> GetUserOrders()
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.GET,
                ApiUrl = $"{ConnectionData.GatewayUrl}/order/GetUserOrders"
            });

            return response;
        }
    }
}
