namespace MessageService_API.Messages
{
    public class GetMoreMessagesData
    {
        public int PageNumber { get; set; }
        public Guid ChatId { get; set; }
    }
}