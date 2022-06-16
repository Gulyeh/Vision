using OrderService_API.Helpers;

namespace OrderService_API.Services.IServices
{
    public interface IProductsService
    {
        Task<T?> CheckProductExists<T>(Guid productId, string Access_Token, OrderType orderType, Guid? gameId = null);
    }
}