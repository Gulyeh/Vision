using System.Web;

namespace PaymentService_API.Messages
{
    public class PaymentCompleted
    {
        public PaymentCompleted()
        {
            Email = string.Empty;
        }

        public bool isSuccess { get; set; }
        public Guid userId { get; set; }
        public Guid PaymentId { get; set; }
        public string Email { get; set; }
        public Guid OrderId { get; set; }
        private string access_Token = string.Empty;
        public string Access_Token
        {
            get => access_Token;
            set => access_Token = $"Bearer {HttpUtility.UrlDecode(value)}";
        }
    }
}