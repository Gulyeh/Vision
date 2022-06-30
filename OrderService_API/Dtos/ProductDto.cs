using OrderService_API.Helpers;

namespace OrderService_API.Dtos
{
    public class ProductDto : BaseProductData
    {
        public Guid ProductId { get; set; }
    }
}