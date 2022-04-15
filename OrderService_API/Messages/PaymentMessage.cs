namespace OrderService_API.Messages
{
    public class PaymentMessage
    {
        public PaymentMessage(decimal Price, decimal Discount, decimal CouponDiscount)
        {
            Title = string.Empty;
            Email = string.Empty;
            TotalPrice = (Price - Discount) * (CouponDiscount/100);
        }

        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public decimal TotalPrice { get; private init; }
        public string Title { get; set; }
    }
}