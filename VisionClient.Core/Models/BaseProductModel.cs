using VisionClient.Core.Helpers;

namespace VisionClient.Core.Models
{
    public class BaseProductModel : NotifyPropertyChanged
    {
        public BaseProductModel()
        {
            Title = string.Empty;
            Details = string.Empty;
            PhotoUrl = string.Empty;
        }

        public string Title { get; set; }
        private decimal price;
        public decimal Price
        {
            get => price;
            set
            {
                if (value < 0) price = 0;
                else price = Math.Round(value, 2);
                OnPropertyChanged();
            }
        }
        private decimal? oldPrice;
        public decimal? OldPrice
        {
            get => oldPrice;
            set
            {
                var parsed = decimal.TryParse(value.ToString(), out decimal parsedValue);
                if (parsed)
                {
                    oldPrice = Math.Round(parsedValue, 2);
                    OnPropertyChanged();
                }
            }
        }
        public bool IsAvailable { get; set; }
        private int? discount;
        public int? Discount
        {
            get => discount;
            set
            {
                discount = value;
                Discounted();
            }
        }
        public string Details { get; set; }
        public string PhotoUrl { get; set; }

        public virtual void Discounted()
        {
            if (Discount > 0)
            {
                OldPrice = Price;
                Price = Math.Round(Price - (Price * (decimal)Discount / 100), 2);
            }
        }
    }
}
