using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionClient.Core.Models
{
    public class PurchaseModel
    {
        public PurchaseModel()
        {
            Title = string.Empty;
            PhotoUrl = string.Empty;
            Details = new List<string>();
            DiscountText = string.Empty;
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string PhotoUrl { get; set; }
        public int? Discount { get; set; }
        public string DiscountText { get; private set; }
        public decimal Price { get; set; }
        public List<string> Details { get; set; }

        public void CreateDiscountText()
        {
            if (Discount > 0) DiscountText = $"-{Discount}%";
        }
    }
}
