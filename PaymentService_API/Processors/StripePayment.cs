using PaymentService_API.Messages;
using PaymentService_API.Processors.Interfaces;
using PaymentService_API.Services.IServices;

namespace PaymentService_API.Processors
{
    public class StripePayment : IPaymentProvider
    {
        private readonly IStripeService stripeService;

        public StripePayment(IStripeService stripeService)
        {
            this.stripeService = stripeService;
        }

        public async Task<string> GeneratePaymentUrl(PaymentMessage data)
        {
            return await stripeService.GeneratePayment(data);
        }
    }
}