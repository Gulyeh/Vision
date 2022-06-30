using VisionClient.Core.Helpers;

namespace VisionClient.Core.Dtos
{
    public class AddCurrencyDto : NotifyPropertyChanged
    {
        public AddCurrencyDto()
        {
            Title = string.Empty;
            Details = string.Empty;
        }

        public string Title { get; set; }
        public bool IsAvailable { get; set; }
        private decimal price;
        public decimal Price
        {
            get => price;
            set
            {
                if (value < 1 || value > 9999) price = 1;
                price = Math.Round(value, 2);
                OnPropertyChanged();
            }
        }
        private int discount;
        public int Discount
        {
            get => discount;
            set
            {
                if (value <= 100 && value >= 0) discount = value;
                else discount = 0;
                OnPropertyChanged();
            }
        }
        public string Details { get; set; }
        public int Amount { get; set; }

        public bool Validator() => !string.IsNullOrWhiteSpace(Title) && Price > 0 && Amount > 0 && Discount <= 100 && Discount >= 0;
    }
}
