namespace OrderService_API.Messages
{
    public class PaymentCompleted
    {
        public PaymentCompleted()
        {
            Email = string.Empty;
        }

        public bool isSuccess { get; set; }
        public Guid userId { get; set; }
        public Guid orderId { get; set; }
        public string Email { get; set; }
        public string? Access_Token { get; set; }
    }
}