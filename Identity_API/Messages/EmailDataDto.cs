using Identity_API.Helpers;
using Identity_API.Messages.Interfaces;

namespace Identity_API.Dtos
{
    public class EmailDataDto : IEmailDataDto
    {
        public EmailDataDto()
        {
            Content = string.Empty;
            ReceiverEmail = string.Empty;
        }

        public Guid UserId { get; set; }
        public string Content { get; set; }
        public string ReceiverEmail { get; set; }
        public EmailTypes EmailType { get; set; }
    }
}