namespace MessageService_API.Messages
{
    public class ChatUsers
    {
        public ChatUsers(Guid senderId, Guid receiverId)
        {
            SenderId = senderId;
            ReceiverId = receiverId;
        }

        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
    }
}