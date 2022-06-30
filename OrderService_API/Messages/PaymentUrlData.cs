namespace OrderService_API.Messages
{
    public class PaymentUrlData
    {
        public PaymentUrlData()
        {
            PaymentUrl = string.Empty;
        }

        public Guid userId { get; set; }
        public string PaymentUrl { get; set; }
    }
}