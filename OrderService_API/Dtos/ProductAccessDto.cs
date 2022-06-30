namespace OrderService_API.Dtos
{
    public class ProductAccessDto
    {
        public ProductAccessDto(bool isSuccess, Guid gameId, Guid productId)
        {
            IsSuccess = isSuccess;
            GameId = gameId;
            ProductId = productId;
        }

        public bool IsSuccess { get; private set; }
        public Guid GameId { get; private set; }
        public Guid ProductId { get; private set; }
    }
}