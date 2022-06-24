using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Helpers;

namespace VisionClient.Core.Dtos
{
    public class EditPackageDto : NotifyPropertyChanged
    {
        public EditPackageDto()
        {
            Title = string.Empty;
            Details = string.Empty;
            Photo = string.Empty;
        }

        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
        public string Photo { get; set; }
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

        public bool Validator() => Id != Guid.Empty && !string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Details) && Price > 0 && Discount <= 100 && Discount >= 0;
    }
}
