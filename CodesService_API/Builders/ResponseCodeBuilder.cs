using CodesService_API.Helpers;

namespace CodesService_API.Builders
{
    public class ResponseCodeBuilder
    {
        private ResponseCode response = new();
        public ResponseCode Build()
        {
            return response;
        }

        public void SetSignature(Signatures? signature)
        {
            if (signature is null) return;
            var signatureName = Enum.GetName(typeof(Signatures), signature);
            response.Signature = string.IsNullOrWhiteSpace(signatureName) ? string.Empty : signatureName;
        }

        public void SetCodeValue(string CodeValue)
        {
            response.CodeValue = CodeValue;
        }

        public void SetGame(Guid? GameId)
        {
            response.GameId = GameId;
        }

        public void SetCodeType(CodeTypes codeType)
        {
            response.CodeType = codeType;
        }

        public void SetCoupon(string coupon)
        {
            response.Coupon = coupon;
        }
    }
}