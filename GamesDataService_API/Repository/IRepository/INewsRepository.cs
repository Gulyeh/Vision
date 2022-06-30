using GamesDataService_API.Dtos;

namespace GamesDataService_API.Repository.IRepository
{
    public interface INewsRepository
    {
        Task<ResponseDto> GetGameNews(Guid gameId);
        Task<ResponseDto> AddNews(AddNewsDto data);
        Task<ResponseDto> EditNews(EditNewsDto data);
        Task<ResponseDto> DeleteNews(Guid newsId, Guid gameId);
    }
}