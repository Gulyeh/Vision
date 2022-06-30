namespace UsersService_API.Messages
{
    public class UserGameDto
    {
        public bool IsSuccess { get; set; }
        public Guid UserId { get; set; }
        public Guid GameId { get; set; }
        public Guid ProductId { get; set; }
    }
}