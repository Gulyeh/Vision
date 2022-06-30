using Identity_API.Helpers;

namespace Identity_API.Messages.Interfaces
{
    public interface IEmailDataDto
    {
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public string ReceiverEmail { get; set; }
        public EmailTypes EmailType { get; set; }
    }
}