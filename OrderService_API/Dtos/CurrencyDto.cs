using OrderService_API.Helpers;

namespace OrderService_API.Dtos
{
    public class CurrencyDto : BaseProductData
    {
        public Guid Id { get; set; }
        public int Amount { get; set; }
    }
}