using VisionClient.Core.Enums;

namespace VisionClient.Core.Models
{
    public class CouponModel
    {
        public CouponModel()
        {
            Coupon = string.Empty;
            CodeValue = string.Empty;
            Signature = string.Empty;
        }

        public string Coupon { get; set; }
        public string CodeValue { get; set; }
        public string Signature { get; set; }
        public Guid? GameId { get; set; }
        public CodeTypes CodeType { get; set; }
    }
}
