namespace OrderService_API.Dtos
{
    public class ProductDto
    {
        public ProductDto()
        {
            Title = string.Empty;
        }

        public Guid ProductId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public int? Discount { get; set; }
    }
}