using VisionClient.Core.Dtos;
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

        public async Task<string> AddCoupon(AddCouponDto data)
        {
            var response = await couponService.AddCoupon(data);
            if (response is null) throw new Exception();
            return ResponseToJsonHelper.GetJson(response);
        }

        public async Task<string> ApplyCoupon(string coupon, CodeTypes type)
        {
            if (string.IsNullOrEmpty(coupon)) return "Field cannot be empty";
            var response = await couponService.ApplyCoupon(coupon, type);
            if (response.isSuccess) return string.Empty;
            return ResponseToJsonHelper.GetJson(response);
        }

        public async Task<(bool, string)> DeleteCoupon(string coupon)
        {
            var response = await couponService.DeleteCoupon(coupon);
            if (response is null) throw new Exception();
            return (response.isSuccess, ResponseToJsonHelper.GetJson(response));
        }

        public async Task<(bool, string)> DeleteUsedCoupon(Guid couponId)
        {
            var response = await couponService.DeleteUsedCoupon(couponId);
            if (response is null) throw new Exception();
            return (response.isSuccess, ResponseToJsonHelper.GetJson(response));
        }

        public async Task<List<DetailedCouponModel>> GetCoupons()
        {
            var response = await couponService.GetCoupons();
            if (response is null) throw new Exception();
            return ResponseToJsonHelper.GetJson<List<DetailedCouponModel>>(response);
        }

        public async Task<List<UsedCodeModel>> GetUserUsedCoupons(Guid userId)
        {
            var response = await couponService.GetUserUsedCoupons(userId);
            if (response is null) throw new Exception();
            return ResponseToJsonHelper.GetJson<List<UsedCodeModel>>(response);
        }

        public async Task<(bool, string)> UpdateCoupon(EditCouponDto data)
        {
            var response = await couponService.UpdateCoupon(data);
            if (response is null) throw new Exception();
            return (response.isSuccess, ResponseToJsonHelper.GetJson(response));
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
