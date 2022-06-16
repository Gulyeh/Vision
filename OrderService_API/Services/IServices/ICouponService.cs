using OrderService_API.Dtos;
using OrderService_API.Helpers;

namespace OrderService_API.Services.IServices
{
    public interface ICouponService
    {
        Task<CouponDataDto> ApplyCoupon(string coupon, string Access_Token, CodeTypes codeType);
    }
}