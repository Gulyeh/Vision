using VisionClient.Core.Models;

namespace VisionClient.Core.Repository.IRepository
{
    public interface IOrderRepository
    {
        Task<List<OrderModel>> GetOrders(string? orderId = null);
        Task<(bool, string)> ChangeOrderToPaid(Guid orderId);
        Task<List<OrderModel>> GetUserOrders();
    }
}
