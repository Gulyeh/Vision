namespace VisionClient.Core.Dtos
{
    public class MessageDto
    {
        public MessageDto()
        {
            Content = string.Empty;
            AttachmentsList = new List<string>();
        }

        public string Content { get; set; }
        public Guid ChatId { get; set; }
        public List<string> AttachmentsList { get; set; }
    }
}
