namespace CodesService_API.Messages
{
    public class ProductDto
    {
        public ProductDto(Guid userId, Guid gameId, Guid productId, string code)
        {
            UserId = userId;
            GameId = gameId;
            ProductId = productId;
            Code = code;
        }

        public Guid UserId { get; set; }
        public Guid GameId { get; set; }
        public Guid ProductId { get; set; }
        public string Code { get; set; }
    }
}