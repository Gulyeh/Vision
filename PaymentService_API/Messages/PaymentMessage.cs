namespace PaymentService_API.Messages
{
    public class PaymentMessage
    {
        public PaymentMessage()
        {
            Title = string.Empty;
            Email = string.Empty;
            Access_Token = string.Empty;
        }

        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public Guid PaymentMethodId { get; set; }
        public string Email { get; set; }
        public decimal TotalPrice { get; set; }
        public string Title { get; set; }
        private string access_Token = string.Empty;
        public string Access_Token
        {
            get => access_Token;
            set => access_Token = value.Replace("Bearer ", "");
        }
    }
}