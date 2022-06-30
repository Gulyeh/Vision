using OrderService_API.Helpers;

namespace OrderService_API.Messages
{
    public class ApplyCouponDto
    {
        public ApplyCouponDto(string coupon, CodeTypes codeType, Guid userId)
        {
            UserId = userId;
            Coupon = coupon;
            var typeName = Enum.GetName(typeof(CodeTypes), codeType);
            CodeType = string.IsNullOrWhiteSpace(typeName) ? string.Empty : typeName;
        }

        public string Coupon { get; init; }
        public string CodeType { get; init; }
        public Guid UserId { get; init; }
    }
}