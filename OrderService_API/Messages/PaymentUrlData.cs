using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService_API.Messages
{
    public class PaymentUrlData
    {
        public PaymentUrlData()
        {
            PaymentUrl = string.Empty;
        }

        public Guid userId { get; set; }
        public string PaymentUrl { get; set; }
    }
}