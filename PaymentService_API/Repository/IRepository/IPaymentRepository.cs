using PaymentService_API.Dtos;
using PaymentService_API.Helpers;
using PaymentService_API.Messages;

namespace PaymentService_API.Repository.IRepository
{
    public interface IPaymentRepository
    {
        Task<ResponseDto> GetPaymentMethods();
        Task CreatePayment(PaymentMessage data);
        Task<PaymentUrlData> RequestPayment(PaymentMessage data);
        Task<ResponseDto> PaymentCompleted(string sessionId, PaymentStatus status, string Access_Token);
        Task<IEnumerable<string>> GetNewProviders();
        Task<ResponseDto> AddPaymentMethod(AddPaymentMethodDto data);
        Task<ResponseDto> DeletePaymentMethod(Guid paymentId);
        Task<ResponseDto> UpdatePaymentMethod(EditPaymentMethodDto data);
        Task<ResponseDto> GetUserPayments(Guid userId);
    }
}