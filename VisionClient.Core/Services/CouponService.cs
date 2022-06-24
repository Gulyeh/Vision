using VisionClient.Core.Dtos;
using VisionClient.Core.Enums;
using VisionClient.Core.Helpers;
using VisionClient.Core.Services.IServices;

namespace VisionClient.Core.Services
{
    public class CouponService : HttpService, ICouponService
    {
        public CouponService(IHttpClientFactory httpClientFactory, IStaticData staticData) : base(httpClientFactory, staticData)
        {
        }

        public async Task<ResponseDto?> AddCoupon(AddCouponDto data)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.POST,
                ApiUrl = $"{ConnectionData.GatewayUrl}/codes/AddCode",
                Data = data
            });

            if (response is not null) return response;
            return null;
        }

        public async Task<ResponseDto> ApplyCoupon(string coupon, CodeTypes type)
        {
            var codeType = Enum.GetName(typeof(CodeTypes), type);
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.PUT,
                ApiUrl = $"{ConnectionData.GatewayUrl}/codes/ApplyCode?code={coupon}&codeType={codeType}"
            });

            if (response is not null) return response;
            return new ResponseDto();
        }

        public async Task<ResponseDto?> DeleteCoupon(string coupon)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.DELETE,
                ApiUrl = $"{ConnectionData.GatewayUrl}/codes?code={coupon}"
            });

            if (response is not null) return response;
            return null;
        }

        public async Task<ResponseDto?> GetCoupons()
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.GET,
                ApiUrl = $"{ConnectionData.GatewayUrl}/codes"
            });

            if (response is not null) return response;
            return null;
        }

        public async Task<ResponseDto?> UpdateCoupon(EditCouponDto data)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.PUT,
                ApiUrl = $"{ConnectionData.GatewayUrl}/codes/editcode",
                Data = data
            });

            if (response is not null) return response;
            return null;
        }

        public async Task<ResponseDto> VerifyCoupon(string coupon, CodeTypes type)
        {
            var codeType = Enum.GetName(typeof(CodeTypes), type);
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.GET,
                ApiUrl = $"{ConnectionData.GatewayUrl}/codes/ValidateCode?code={coupon}&codeType={codeType}"
            });

            if (response is not null) return response;
            return new ResponseDto();
        }
    }
}
