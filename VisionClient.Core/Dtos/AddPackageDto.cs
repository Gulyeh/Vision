using VisionClient.Core.Helpers;

namespace VisionClient.Core.Dtos
{
    public class AddPackageDto : NotifyPropertyChanged
    {
        public AddPackageDto()
        {
            Title = string.Empty;
            Details = string.Empty;
            Photo = string.Empty;
        }

        public string Title { get; set; }
        public Guid GameId { get; set; }
        public string Photo { get; set; }
        public string Details { get; set; }
        public bool IsAvailable { get; set; }
        private decimal price;
        public decimal Price
        {
            get => price;
            set
            {
                if (value < 0 || value > 999) price = 0;
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

        public bool Validator() => Price > 0 && !string.IsNullOrWhiteSpace(Details) && !string.IsNullOrWhiteSpace(Title);
    }
}
