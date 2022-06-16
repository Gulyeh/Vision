using PaymentService_API.Helpers;
using PaymentService_API.Processors.Interfaces;
using PaymentService_API.Services.IServices;

namespace PaymentService_API.Processors
{
    public class PaymentUrlProcessor : IPaymentProcessor
    {
        private readonly IStripeService stripeService;

        public PaymentUrlProcessor(IStripeService stripeService)
        {
            this.stripeService = stripeService;
        }

        public IPaymentProvider GetProvider(PaymentProvider provider)
        {
            return provider switch
            {
                PaymentProvider.Stripe => new StripePayment(stripeService),
                _ => new StripePayment(stripeService)
            };
        }
    }
}