using VisionClient.Core.Dtos;
using VisionClient.Core.Enums;
using VisionClient.Core.Models;

namespace VisionClient.Core.Repository.IRepository
{
    public interface ICouponRepository
    {
        Task<(CouponModel, string?)> VerifyCoupon(string coupon, CodeTypes type);
        Task<string> ApplyCoupon(string coupon, CodeTypes type);
        Task<string> AddCoupon(AddCouponDto data);
        Task<List<DetailedCouponModel>> GetCoupons();
        Task<(bool, string)> DeleteCoupon(string coupon);
        Task<(bool, string)> UpdateCoupon(EditCouponDto data);
    }
}
