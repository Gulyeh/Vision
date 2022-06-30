using VisionClient.Core.Dtos;
using VisionClient.Core.Enums;

namespace VisionClient.Core.Services.IServices
{
    public interface ICouponService
    {
        Task<ResponseDto> ApplyCoupon(string coupon, CodeTypes type);
        Task<ResponseDto> VerifyCoupon(string coupon, CodeTypes type);
        Task<ResponseDto?> AddCoupon(AddCouponDto data);
        Task<ResponseDto?> GetCoupons();
        Task<ResponseDto?> DeleteCoupon(string coupon);
        Task<ResponseDto?> UpdateCoupon(EditCouponDto data);
        Task<ResponseDto?> DeleteUsedCoupon(Guid couponId);
        Task<ResponseDto?> GetUserUsedCoupons(Guid userId);
    }
}
