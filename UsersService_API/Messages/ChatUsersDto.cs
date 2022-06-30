namespace UsersService_API.Messages
{
    public class ChatUsersDto
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
    }
}