using ProdcutsService_API.Helpers;

namespace ProdcutsService_API.Messages
{
    public class GetProductDto
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public Guid GameId { get; set; }
        private OrderType? orderType;
        public OrderType? OrderType
        {
            get => orderType;
            set
            {
                var isParsed = Enum.TryParse(value.ToString(), out OrderType type);
                if (isParsed) orderType = type;
            }
        }
    }
}