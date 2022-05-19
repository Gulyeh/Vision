using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionClient.Core.Models
{
    public class PaymentMethod
    {
        public PaymentMethod()
        {
            PaymentName = string.Empty;
            PhotoUrl = string.Empty;
        }

        public string PaymentName { get; set; }
        public string PhotoUrl { get; set; }
        public bool IsAvailable { get; set; }
    }
}
