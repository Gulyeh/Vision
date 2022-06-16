using VisionClient.Core.Models;

namespace VisionClient.Core.Repository.IRepository
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<PaymentMethod>> GetPaymentMethods();
    }
}
