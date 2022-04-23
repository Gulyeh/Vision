using PaymentService_API.Helpers;
using PaymentService_API.Messages;

namespace PaymentService_API.Repository.IRepository
{
    public interface IPaymentRepository
    {
        Task CreatePayment(PaymentMessage data);
        Task<PaymentUrlData> RequestStripePayment(PaymentMessage data);
        Task<bool> PaymentCompleted(string sessionId, PaymentStatus status, string? Access_Token = null);
    }
}