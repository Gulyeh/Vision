﻿using VisionClient.Core.Dtos;
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
