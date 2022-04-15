namespace UsersService_API.Messages
{
    public class UserGameDto
    {
        public Guid userId { get; set; }
        public Guid gameId { get; set; }
        public Guid? productId { get; set; }
    }
}