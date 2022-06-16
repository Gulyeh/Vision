using OrderService_API.Helpers;

namespace OrderService_API.Services.IServices
{
    public interface IGameAccessService
    {
        Task<bool> CheckProductAccess(CreateOrderData data);
    }
}