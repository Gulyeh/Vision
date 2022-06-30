using OrderService_API.Helpers;

namespace OrderService_API.Dtos
{
    public class GameDto : BaseProductData
    {
        public Guid GameId { get; set; }
        public ICollection<ProductDto>? GameProducts { get; set; }
    }
}