using ProductsService_API.Dtos;

namespace ProductsService_API.Repository.IRepository
{
    public interface IGamesRepository
    {
        Task<ResponseDto> AddGame(AddGamesDto data, string Access_Token);
        Task<ResponseDto> DeleteGame(Guid gameId);
        Task<ResponseDto> EditGame(GamesDto data);
        Task<ResponseDto> GetGame(Guid gameId, string Access_Token);
    }
}