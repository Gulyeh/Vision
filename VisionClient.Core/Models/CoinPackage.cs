using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionClient.Core.Models
{
    public class CoinPackage
    {
        public CoinPackage()
        {
            Title = string.Empty;
            DiscountText = string.Empty;
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public int Amount { get; set; }
        public int? Discount { get; set; }
        public string DiscountText { get; private set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public bool IsAvailable { get; set; }

        public void Discounted()
        {
            if(Discount > 0)
            {
                OldPrice = Price;
                Price -= Price * ((decimal)Discount / 100);
                DiscountText = $"-{Discount}%";
            }
        }
    }
}
