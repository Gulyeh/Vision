namespace OrderService_API.Helpers
{
    public class BaseProductData
    {
        public BaseProductData()
        {
            Title = string.Empty;
            Discount = 0;
        }

        public string Title { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsPurchased { get; set; } = false;
        public decimal Discount { get; set; }
    }
}