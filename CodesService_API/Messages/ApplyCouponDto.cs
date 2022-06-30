using CodesService_API.Helpers;

namespace CodesService_API.Messages
{
    public class ApplyCouponDto
    {
        public ApplyCouponDto()
        {
            Coupon = string.Empty;
        }

        public string Coupon { get; set; }
        private CodeTypes codeType;
        public CodeTypes CodeType
        {
            get => codeType;
            set
            {
                var isParsed = Enum.TryParse(value.ToString(), true, out CodeTypes type);
                if (isParsed) codeType = type;
                else codeType = CodeTypes.Discount;
            }
        }
        public Guid UserId { get; set; }
    }
}