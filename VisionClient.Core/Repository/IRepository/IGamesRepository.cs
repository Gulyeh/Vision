using VisionClient.Core.Models;

namespace VisionClient.Core.Repository.IRepository
{
    public interface IGamesRepository
    {
        Task GetGames();
        Task<IEnumerable<NewsModel>> GetNews(Guid gameId);
        Task<GameProductModel> GetProducts(Guid gameId);
        Task<bool> OwnsProduct(Guid productId, Guid gameId);
        Task<BanModel?> CheckGameAccess(Guid gameId);
    }
}
