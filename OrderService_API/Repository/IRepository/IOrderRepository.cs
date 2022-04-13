using OrderService_API.Dtos;
using OrderService_API.Messages;

namespace OrderService_API.Repository.IRepository
{
    public interface IOrderRepository
    {
        Task<PaymentMessage?> CreateOrder(Guid gameId, Guid userId, string Email, string Access_Token, Guid? productId = null);
        Task<ResponseDto> GetOrders(Guid productId, Guid? userId = null);
        Task<OrderDto> GetOrder(Guid orderId);
        Task<ResponseDto> DeleteOrder(Guid orderId);
        Task ChangeOrderStatus(Guid orderId, bool isPaid);
    }
}