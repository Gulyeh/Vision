using ProductsService_API.Dtos;

namespace ProductsService_API.Services.IServices
{
    public interface IGameDataService
    {
        Task<ResponseDto> CheckGameExists(Guid gameId, string Access_Token);
    }
}