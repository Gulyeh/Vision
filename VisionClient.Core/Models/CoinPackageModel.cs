using VisionClient.Core.Helpers;

namespace VisionClient.Core.Models
{
    public class CoinPackageModel : NotifyPropertyChanged
    {
        public CoinPackageModel()
        {
            Title = string.Empty;
            DiscountText = string.Empty;
            Details = string.Empty;
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public int Amount { get; set; }
        private int discount;
        public int Discount
        {
            get => discount;
            set
            {
                discount = value;
                Discounted();
            }
        }
        public string DiscountText { get; private set; }
        public string Details { get; set; }
        private decimal price;
        public decimal Price
        {
            get => price;
            set
            {
                if (value <= 0) price = 0;
                else price = value;
                OnPropertyChanged();
            }
        }
        public decimal? OldPrice { get; set; }
        public bool IsAvailable { get; set; }

        private void Discounted()
        {
            if (Discount > 0)
            {
                OldPrice = Price;
                Price -= Price * ((decimal)Discount / 100);
                DiscountText = $"-{Discount}%";
            }
        }
    }
}
