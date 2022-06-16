namespace VisionClient.Core.Dtos
{
    public class GetMoreMessagesDto
    {
        public GetMoreMessagesDto(int pageNumber, Guid chatId)
        {
            PageNumber = pageNumber;
            ChatId = chatId;
        }

        public int PageNumber { get; set; }
        public Guid ChatId { get; set; }
    }
}
