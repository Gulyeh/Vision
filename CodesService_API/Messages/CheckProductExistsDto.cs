using CodesService_API.Helpers;

namespace CodesService_API.Messages
{
    public class CheckProductExistsDto
    {
        public CheckProductExistsDto(Guid productId, Guid gameId, OrderType type)
        {
            ProductId = productId;
            GameId = gameId;
            var orderName = Enum.GetName(typeof(OrderType), type);
            OrderType = string.IsNullOrWhiteSpace(orderName) ? string.Empty : orderName;
        }

        public Guid ProductId { get; init; }
        public Guid GameId { get; init; }
        public string OrderType { get; init; }
    }
}