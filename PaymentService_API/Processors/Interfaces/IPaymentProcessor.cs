using PaymentService_API.Helpers;

namespace PaymentService_API.Processors.Interfaces
{
    public interface IPaymentProcessor
    {
        IPaymentProvider GetProvider(PaymentProvider provider);
    }
}