namespace VisionClient.Core.Models
{
    public class GameModel
    {
        public GameModel()
        {
            Title = string.Empty;
            IconUrl = string.Empty;
            PhotoUrl = string.Empty;
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string IconUrl { get; set; }
        public string PhotoUrl { get; set; }
        public bool IsPurchased { get; set; }
        public IEnumerable<NewsModel>? News { get; set; }
        public IEnumerable<ProductsModel>? Products { get; set; }
    }
}
