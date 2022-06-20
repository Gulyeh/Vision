using VisionClient.Core.Dtos;
using VisionClient.Core.Models;

namespace VisionClient.Core.Repository.IRepository
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<PaymentMethod>> GetPaymentMethods();
        Task<IEnumerable<string>> GetNewMethods();
        Task<(bool, string)> AddPaymentMethod(AddPaymentMethodDto data);
    }
}
