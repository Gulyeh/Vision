using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OrderService_API.Dtos;
using OrderService_API.Helpers;
using OrderService_API.Services.IServices;
using OrderService_API.Statics;

namespace OrderService_API.Services
{
    public class CouponService : BaseHttpService, ICouponService
    {
        public CouponService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        private class Response{
            public int codeValue { get; set; }
        }

        public async Task<int> ApplyCoupon(string coupon, string Access_Token, CodeTypes codeType)
        {
           var response = await SendAsync<Response>(new ApiRequest()
            {
                apiType = APIType.GET,
                ApiUrl = $"{APIUrls.CouponServiceUrl}api/codes/applycode?code={coupon}&codeType={codeType}",
                Access_Token = Access_Token
            });

            if(response is not null) return response.codeValue;
            return 0;
        }
    }
}