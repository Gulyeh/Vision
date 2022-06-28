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
        Task<ResponseDto?> AddNews(AddNewsDto data);
        Task<ResponseDto?> AddGamePackage(AddPackageDto data);
        Task<ResponseDto?> EditGame(EditGameDto data);
        Task<ResponseDto?> DeleteNews(Guid gameId, Guid newsId);
        Task<ResponseDto?> EditNews(EditNewsDto data);
        Task<ResponseDto?> DeleteGame(Guid gameId);
        Task<ResponseDto?> DeletePackage(Guid packageId, Guid gameId);
        Task<ResponseDto?> EditPackage(EditPackageDto data);
        Task<ResponseDto?> BanUser(BanGameDto data);
        Task<ResponseDto?> UnbanUser(Guid userId, Guid gameId);
        Task<ResponseDto?> CheckIfUserIsBanned(Guid userId, Guid gameId);
        Task<ResponseDto?> GiveUserProduct(GiveProductDto data);
    }
}
