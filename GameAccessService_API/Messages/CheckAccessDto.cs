namespace GameAccessService_API.Messages
{
    public class CheckAccessDto
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public Guid GameId { get; set; }
    }
}