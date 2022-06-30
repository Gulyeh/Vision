using ProdcutsService_API.Helpers;

namespace ProductsService_API.Messages
{
    public class CheckProductExistsDto
    {
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