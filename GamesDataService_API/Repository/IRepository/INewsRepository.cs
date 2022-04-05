using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamesDataService_API.Dtos;

namespace GamesDataService_API.Repository.IRepository
{
    public interface INewsRepository
    {
        Task<ResponseDto> GetGameNews(Guid gameId);
        Task<ResponseDto> AddNews(AddNewsDto data);
        Task<ResponseDto> EditNews(NewsDto data);
        Task<ResponseDto> DeleteNews(Guid newsId, Guid gameId);
    }
}