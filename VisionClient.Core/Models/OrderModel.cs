using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Helpers;

namespace VisionClient.Core.Models
{
    public class OrderModel : NotifyPropertyChanged
    {
        public OrderModel()
        {
            Title = string.Empty;
            CouponCode = string.Empty;
            OrderType = string.Empty;
            PaymentUrl = string.Empty;
        }

        public Guid Id { get; set; }
        public string Title { get; set; }

        private DateTime orderDate;
        public DateTime OrderDate
        {
            get => orderDate;
            set => orderDate = value.ToLocalTime();
        }

        private bool paid;
        public bool Paid 
        {
            get => paid; 
            set
            {
                paid = value;
                OnPropertyChanged();
            }
        }
        public string OrderType { get; set; }
        public string CouponCode { get; set; }
        public decimal Price { get; set; }
        public string PaymentUrl { get; set; }

    }
}
