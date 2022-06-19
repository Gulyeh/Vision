using VisionClient.Core.Dtos;

namespace VisionClient.Core.Services.IServices
{
    public interface IGamesService
    {
        Task<ResponseDto?> GetGames();
        Task<ResponseDto?> GetNews(Guid gameId);
        Task<ResponseDto?> GetProducts(Guid gameId);
        Task<ResponseDto?> BoughtPackage(Guid productId, Guid gameId);
        Task<ResponseDto?> CheckGameAccess(Guid gameId);
        Task<ResponseDto?> AddGame(AddGameDto data);
    }
}
