namespace OrderService_API.Messages
{
    public class UserAccessDto
    {
        public bool IsSuccess { get; set; }
        public Guid GameId { get; set; }
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
    }
}