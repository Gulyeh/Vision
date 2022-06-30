using ProductsService_API.Dtos;
using ProductsService_API.Entites;
using ProductsService_API.Messages;

namespace ProductsService_API.Repository.IRepository
{
    public interface IGamesRepository
    {
        Task AddGame(NewProductDto data);
        Task<bool> DeleteGame(Guid gameId);
        Task<ResponseDto> EditGame(EditPackageDto data);
        Task<ResponseDto> GetGame(Guid gameId, Guid userId);
        Task UpdateGameData(GameProductData data);
        Task<Games?> FindGame(Guid gameId);
    }
}