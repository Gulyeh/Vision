using OrderService_API.Dtos;
using OrderService_API.Helpers;
using OrderService_API.Messages;

namespace OrderService_API.Repository.IRepository
{
    public interface IOrderRepository
    {
        Task<PaymentMessage?> CreateOrder<T>(CreateOrderData data, T product) where T : BaseProductData;
        Task<ResponseDto> GetUserOrders(Guid userId);
        Task<ResponseDto> GetOrders(string orderId);
        Task<OrderDto> GetOrder(Guid orderId);
        Task<bool> ChangeOrderStatus(Guid orderId, bool isPaid, Guid? paymentId = null);
    }
}