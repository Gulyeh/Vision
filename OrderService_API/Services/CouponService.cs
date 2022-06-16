using OrderService_API.Dtos;
using OrderService_API.Helpers;
using OrderService_API.Services.IServices;

namespace OrderService_API.Services
{
    public class CouponService : BaseHttpService, ICouponService
    {
        private readonly string CouponServiceUrl;

        public CouponService(IHttpClientFactory httpClientFactory, ILogger<BaseHttpService> logger, IConfiguration config) : base(httpClientFactory, logger)
        {
            CouponServiceUrl = config.GetValue<string>("CouponServiceUrl");
        }

        public async Task<CouponDataDto> ApplyCoupon(string coupon, string Access_Token, CodeTypes codeType)
        {
            var type = Enum.GetName(typeof(CodeTypes), codeType);
            var response = await SendAsync<CouponDataDto>(new ApiRequest()
            {
                apiType = APIType.PUT,
                ApiUrl = $"{CouponServiceUrl}api/codes/applycode?code={coupon}&codeType={type}",
                Access_Token = Access_Token
            });

            if (response is not null) return response;
            return new();
        }
    }
}