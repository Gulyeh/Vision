namespace OrderService_API.Messages
{
    public class AccessProduct
    {
        public AccessProduct(Guid userId, Guid? gameId, Guid productId, string email)
        {
            UserId = userId;
            GameId = gameId;
            ProductId = productId;
            Email = email;
        }

        public Guid UserId { get; set; }
        public Guid? GameId { get; set; }
        public Guid ProductId { get; set; }
        public string Email { get; set; }
    }
}