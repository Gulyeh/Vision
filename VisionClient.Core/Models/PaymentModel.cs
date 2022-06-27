using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionClient.Core.Models
{
    public class PaymentModel
    {
        public PaymentModel()
        {
            PaymentUrl = string.Empty;
        }

        public Guid OrderId { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentUrl { get; set; }
    }
}
