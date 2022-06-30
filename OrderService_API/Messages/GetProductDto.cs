using OrderService_API.Helpers;

namespace OrderService_API.Messages
{
    public class GetProductDto
    {
        public GetProductDto(Guid productId, Guid? gameId, OrderType orderType, Guid userId)
        {
            UserId = userId;
            ProductId = productId;
            GameId = gameId;
            var orderTypeName = Enum.GetName(typeof(OrderType), orderType);
            OrderType = string.IsNullOrWhiteSpace(orderTypeName) ? string.Empty : orderTypeName;
        }

        public Guid UserId { get; init; }
        public Guid ProductId { get; init; }
        public Guid? GameId { get; init; }
        public string OrderType { get; init; }
    }
}