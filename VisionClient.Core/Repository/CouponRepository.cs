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
            if (string.IsNullOrEmpty(coupon)) return "Field cannot be empty";
            var response = await couponService.ApplyCoupon(coupon, type);
            if (response.isSuccess) return string.Empty;
            return ResponseToJsonHelper.GetJson(response);
        }

        public async Task<(CouponModel, string?)> VerifyCoupon(string coupon, CodeTypes type)
        {
            if (string.IsNullOrEmpty(coupon)) return (new(), "Field cannot be empty");
            var response = await couponService.VerifyCoupon(coupon, type);
            var json = ResponseToJsonHelper.GetJson<CouponModel>(response);
            if (string.IsNullOrWhiteSpace(json.Coupon)) return (json, ResponseToJsonHelper.GetJson(response));       
            return (json, null);
        }
    }
}
