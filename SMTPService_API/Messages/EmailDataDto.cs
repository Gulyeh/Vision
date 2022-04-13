using SMTPService_API.Helpers;

namespace SMTPService_API.Messages
{
    public class EmailDataDto
    {
        public EmailDataDto()
        {
            Content = string.Empty;
            ReceiverEmail = string.Empty;
        }

        public Guid userId { get; set; }
        public string Content { get; set; }
        public string ReceiverEmail { get; set; }
        public EmailTypes EmailType { get; set; }
    }
}