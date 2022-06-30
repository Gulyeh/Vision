namespace UsersService_API.Dtos
{
    public class HasUnreadMessagesDto
    {
        public Guid UserId { get; set; }
        public bool HasUnreadMessages { get; set; }
    }
}