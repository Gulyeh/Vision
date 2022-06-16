using PaymentService_API.Entities;

namespace PaymentService_API.Services.IServices
{
    public interface ICacheService
    {
        Task AddMethodsToCache(PaymentMethods data);
        Task<IEnumerable<PaymentMethods>> GetMethodsFromCache();
        Task RemoveMethodsFromCache(PaymentMethods data);
    }
}