namespace GameAccessService_API.Messages
{
    public class UserCodeDto
    {
        public UserCodeDto()
        {
            Code = string.Empty;
        }

        public bool IsSuccess { get; set; }
        public Guid UserId { get; set; }
        public Guid GameId { get; set; }
        public Guid ProductId { get; set; }
        public string Code { get; set; }
    }
}