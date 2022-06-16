using ProductsService_API.Dtos;

namespace ProductsService_API.Services.IServices
{
    public interface IGameAccessService
    {
        Task<ResponseDto?> CheckGameAccess(Guid gameId, string Access_Token);
        Task<ResponseDto?> CheckProductAccess(Guid productId, Guid gameId, string Access_Token);
    }
}