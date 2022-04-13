using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentService_API.Messages;

namespace PaymentService_API.Services.IServices
{
    public interface IStripeService
    {
        Task<string> GeneratePayment(PaymentMessage data);
    }
}