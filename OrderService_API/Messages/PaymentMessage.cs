using OrderService_API.Dtos;

namespace OrderService_API.Messages
{
    public class PaymentMessage
    {
        public PaymentMessage(decimal Price, decimal Discount, CouponDataDto couponData)
        {
            Title = string.Empty;
            Email = string.Empty;
            Access_Token = string.Empty;
            TotalPrice = Price - (Price * Discount / 100);
            if (couponData.CodeValue > 0)
            {
                if (couponData.Signature.Equals("Procentage")) TotalPrice -= TotalPrice * ((decimal)couponData.CodeValue / 100);
                else if (couponData.Signature.Equals("Price")) TotalPrice -= couponData.CodeValue;
            }
        }

        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public Guid PaymentMethodId { get; set; }
        public string Email { get; set; }
        private decimal totalPrice;
        public decimal TotalPrice
        {
            get => totalPrice;
            private set
            {
                if (value <= 0) totalPrice = 0;
                else totalPrice = Math.Round(value, 2);
            }
        }
        public string Title { get; set; }
        public string Access_Token { get; set; }
    }
}