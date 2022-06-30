namespace UsersService_API.Messages
{
    public class GamePurchased
    {
        public GamePurchased(bool isSuccess, Guid gameId, Guid productId)
        {
            IsSuccess = isSuccess;
            GameId = gameId;
            ProductId = productId;
        }

        public bool IsSuccess { get; set; }
        public Guid GameId { get; set; }
        public Guid ProductId { get; set; }
    }
}