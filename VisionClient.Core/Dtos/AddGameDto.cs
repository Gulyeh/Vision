using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Helpers;
using VisionClient.Core.Models;

namespace VisionClient.Core.Dtos
{
    public class AddGameDto : NotifyPropertyChanged
    {
        public AddGameDto()
        {
            Name = string.Empty;
            IconPhoto = string.Empty;
            CoverPhoto = string.Empty;
            BannerPhoto = string.Empty;
            Details = string.Empty;
            Requirements = new();
            Informations = new();
        }

        public string Name { get; set; }
        public string IconPhoto { get; set; }
        public string CoverPhoto { get; set; }
        public string BannerPhoto { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsPurchasable { get; set; }
        public string Details { get; set; }
        private decimal price;
        public decimal Price 
        {
            get => price;
            set
            {
                if (value <= 0 || value > 999) price = 1;
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
        public RequirementsModel Requirements { get; set; }
        public ProductInfoModel Informations { get; set; }

        public bool Validation() => !string.IsNullOrEmpty(Name) && Informations.Validation() && Requirements.Validation() 
            && Price > 0 && !string.IsNullOrEmpty(Details) && Discount >= 0 && Discount <= 100;      
    }
}
