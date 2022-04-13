namespace MessageService_API.Dtos
{
    public class MessageAttachmentDto
    {
        public MessageAttachmentDto()
        {
            AttachmentUrl = string.Empty;
        }

        public Guid Id { get; set; }
        public string AttachmentUrl { get; set; }
    }
}