using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamesDataService_API.Dtos;

namespace GamesDataService_API.Repository.IRepository
{
    public interface IGamesRepository
    {
        Task<ResponseDto> GetGames();
        Task<ResponseDto> EditGameData(GamesDto data);
        Task<ResponseDto> DeleteGame(Guid gameId);
        Task<ResponseDto> AddGame(AddGamesDto data);
        Task<ResponseDto> CheckGame(Guid gameId);
    }
}