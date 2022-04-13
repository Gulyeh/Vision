using PaymentService_API.Messages;

namespace PaymentService_API.Services.IServices
{
    public interface IStripeService
    {
        Task<string> GeneratePayment(PaymentMessage data);
    }
}