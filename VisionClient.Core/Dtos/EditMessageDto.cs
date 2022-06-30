namespace VisionClient.Core.Dtos
{
    public class EditMessageDto
    {
        public EditMessageDto(Guid messageId, string? content, List<Guid> deletedAttachmentsId, Guid ChatId)
        {
            MessageId = messageId;
            Content = content;
            DeletedAttachmentsId = deletedAttachmentsId;
            this.ChatId = ChatId;
        }

        public Guid ChatId { get; private set; }
        public Guid MessageId { get; private set; }
        public string? Content { get; private set; }
        public List<Guid> DeletedAttachmentsId { get; private set; }
    }
}
