namespace OrderService_API.Messages
{
    public class UserAccessDto : BaseEmailData
    {
        public bool isSuccess { get; set; }
        public Guid gameId { get; set; }
        public Guid? productId { get; set; }
    }
}