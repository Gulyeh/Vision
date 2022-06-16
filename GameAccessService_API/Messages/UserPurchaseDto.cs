namespace GameAccessService_API.Messages
{
    public class UserPurchaseDto
    {
        public UserPurchaseDto()
        {
            Email = string.Empty;
        }

        public bool IsSuccess { get; set; }
        public Guid UserId { get; set; }
        public Guid GameId { get; set; }
        public Guid ProductId { get; set; }
        public string Email { get; set; }
    }
}