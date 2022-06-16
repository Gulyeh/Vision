
using OrderService_API.Helpers;

namespace OrderService_API.Messages
{
    public class EmailDataDto
    {
        public EmailDataDto(Guid userId, string content, string receiverEmail)
        {
            UserId = userId;
            Content = content;
            ReceiverEmail = receiverEmail;
        }

        public Guid UserId { get; set; }
        public string Content { get; set; }
        public string ReceiverEmail { get; set; }
        public EmailTypes EmailType { get; private set; } = EmailTypes.Payment;
    }
}