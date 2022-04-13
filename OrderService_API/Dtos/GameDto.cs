namespace OrderService_API.Dtos
{
    public class GameDto
    {
        public GameDto()
        {
            Title = string.Empty;
        }

        public Guid GameId { get; set; }
        public ICollection<ProductDto>? GameProducts { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public int? Discount { get; set; }
    }
}