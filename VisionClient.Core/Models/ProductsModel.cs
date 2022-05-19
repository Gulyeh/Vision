namespace VisionClient.Core.Models
{
    public class ProductsModel
    {
        public ProductsModel()
        {
            PhotoUrl = string.Empty;
            Title = string.Empty;
        }

        public int Id { get; set; }
        public string PhotoUrl { get; set; }
        public string Title { get; set; }
        public int? Discount { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; private set; }
        public bool IsAvailable { get; set; }

        public void Discounted()
        {
            if (Discount > 0)
            {
                OldPrice = Price;
                Price -= Price * ((decimal)Discount / 100);
            }
        }
    }
}
