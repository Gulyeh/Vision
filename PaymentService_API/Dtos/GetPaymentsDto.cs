using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentService_API.Dtos
{
    public class GetPaymentsDto
    {
        public GetPaymentsDto()
        {
            PaymentUrl = string.Empty;
        }

        public Guid OrderId { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentUrl { get; set; }
    }
}