namespace MessageService_API.Dtos
{
    public class HasUnreadMessagesDto
    {
        public HasUnreadMessagesDto(Guid userId, bool hasUnreadMessages)
        {
            UserId = userId;
            HasUnreadMessages = hasUnreadMessages;
        }

        public Guid UserId { get; private set; }
        public bool HasUnreadMessages { get; private set; }
    }
}