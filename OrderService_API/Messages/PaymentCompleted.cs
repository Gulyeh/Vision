namespace OrderService_API.Messages
{
    public class PaymentCompleted
    {
        public PaymentCompleted()
        {
            Email = string.Empty;
            Access_Token = string.Empty;
        }

        public bool IsSuccess { get; set; }
        public Guid UserId { get; set; }
        public Guid OrderId { get; set; }
        public Guid PaymentId { get; set; }
        public string Email { get; set; }
        public string Access_Token { get; set; }
    }
}