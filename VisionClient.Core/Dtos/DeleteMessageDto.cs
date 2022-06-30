namespace VisionClient.Core.Dtos
{
    public class DeleteMessageDto
    {
        public DeleteMessageDto(Guid messageId, Guid chatId)
        {
            MessageId = messageId;
            ChatId = chatId;
        }

        public Guid ChatId { get; private set; }
        public Guid MessageId { get; private set; }
    }
}
