namespace ProductsService_API.Messages
{
    public class CheckProductAccessDto
    {
        public CheckProductAccessDto(Guid userId, Guid productId, Guid gameId)
        {
            UserId = userId;
            ProductId = productId;
            GameId = gameId;
        }

        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public Guid GameId { get; set; }
    }
}