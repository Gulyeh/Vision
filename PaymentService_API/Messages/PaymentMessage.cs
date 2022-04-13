using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentService_API.Entities;
using PaymentService_API.Helpers;

namespace PaymentService_API.Messages
{
    public class PaymentMessage
    {
        public PaymentMessage()
        {
            Title = string.Empty;
            Email = string.Empty;
        }

        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public decimal TotalPrice { get; set; }
        public string Title { get; set; }
    }
}