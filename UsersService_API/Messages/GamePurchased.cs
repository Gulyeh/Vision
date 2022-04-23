namespace UsersService_API.Messages
{
    public class GamePurchased{
        public Guid gameId { get; set; }
        public Guid? productId { get; set; }
    }
}