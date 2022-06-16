namespace VisionClient.Core.Models
{
    public class GameProductModel : BaseProductModel
    {
        public GameProductModel()
        {
            Products = new List<ProductsModel>();
        }

        public Guid GameId { get; set; }
        private bool isPurchased;
        public bool IsPurchased
        {
            get => isPurchased;
            set
            {
                isPurchased = value;
                OnPropertyChanged();
            }
        }

        public List<ProductsModel> Products { get; set; }
    }
}
