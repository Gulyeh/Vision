using VisionClient.Core.Dtos;
using VisionClient.Core.Models;

namespace VisionClient.Core.Repository.IRepository
{
    public interface IGamesRepository
    {
        Task GetGames();
        Task<IEnumerable<NewsModel>> GetNews(Guid gameId);
        Task<GetPagedNewsDto> GetPagedNews(Guid gameId, int pageNumber);
        Task<GameProductModel> GetProducts(Guid gameId);
        Task<bool> OwnsProduct(Guid productId, Guid gameId);
        Task<BanModel?> CheckGameAccess(Guid gameId);
        Task<(bool, string)> AddGame(AddGameDto data);
        Task<(bool, string)> AddNews(AddNewsDto data);
        Task<(bool, string)> AddGamePackage(AddPackageDto data);
        Task<string> EditGame(EditGameDto data);
        Task<(bool, string)> DeleteNews(Guid gameId, Guid newsId);
        Task<(bool, string)> EditNews(EditNewsDto data);
    }
}
