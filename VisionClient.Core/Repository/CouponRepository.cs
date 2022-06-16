using Newtonsoft.Json;
using VisionClient.Core.Enums;
using VisionClient.Core.Helpers;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Core.Services.IServices;

namespace VisionClient.Core.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ICouponService couponService;

        public CouponRepository(ICouponService couponService)
        {
            this.couponService = couponService;
        }

        public async Task<string> ApplyCoupon(string coupon, CodeTypes type)
        {
            var response = await couponService.ApplyCoupon(coupon, type);
            if (response.isSuccess) return string.Empty;

            var responseString = response.Response.ToString()?.Replace("[", "").Replace("]", "");
            if (!string.IsNullOrWhiteSpace(responseString))
            {
                var responseDeserialized = JsonConvert.DeserializeObject<string>(responseString);
                if (!string.IsNullOrWhiteSpace(responseDeserialized))
                    return responseDeserialized;
            }

            return string.Empty;
        }

        public async Task<(CouponModel, string?)> VerifyCoupon(string coupon, CodeTypes type)
        {
            var response = await couponService.VerifyCoupon(coupon, type);
            var json = ResponseToJsonHelper.GetJson<CouponModel>(response);
            if (string.IsNullOrWhiteSpace(json.Coupon))
            {
                var responseString = response.Response.ToString()?.Replace("[", "").Replace("]", "");
                if (!string.IsNullOrWhiteSpace(responseString))
                {
                    var responseDeserialized = JsonConvert.DeserializeObject<string>(responseString);
                    return (json, responseDeserialized);
                }
            }
            return (json, null);
        }
    }
}
