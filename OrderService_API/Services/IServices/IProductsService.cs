using OrderService_API.Dtos;

namespace OrderService_API.Services.IServices
{
    public interface IProductsService
    {
        Task<T?> CheckProductExists<T>(Guid gameId, string Access_Token, Guid? productId = null);
        Task<GameDto> GetGame(Guid gameId, string Access_Token);
    }
}