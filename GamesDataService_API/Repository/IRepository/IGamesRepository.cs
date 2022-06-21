using GamesDataService_API.Dtos;

namespace GamesDataService_API.Repository.IRepository
{
    public interface IGamesRepository
    {
        Task<ResponseDto> GetGames();
        Task<ResponseDto> EditGameData(EditGameDto data);
        Task<ResponseDto> DeleteGame(Guid gameId);
        Task<ResponseDto> AddGame(AddGamesDto data);
        Task<ResponseDto> CheckGame(Guid gameId);
    }
}