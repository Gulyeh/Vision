using VisionClient.Core.Dtos;

namespace VisionClient.Core.Services.IServices
{
    public interface IOrderService
    {
        Task<ResponseDto?> GetOrders(string? orderId = null);
        Task<ResponseDto?> ChangeOrderToPaid(Guid orderId);
        Task<ResponseDto?> GetUserOrders();
    }
}
