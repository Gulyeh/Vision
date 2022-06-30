using PaymentService_API.Messages;

namespace PaymentService_API.Processors.Interfaces
{
    public interface IPaymentProvider
    {
        Task<string> GeneratePaymentUrl(PaymentMessage data);
    }
}