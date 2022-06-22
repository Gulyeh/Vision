﻿using VisionClient.Core.Dtos;

namespace VisionClient.Core.Services.IServices
{
    public interface IGamesService
    {
        Task<ResponseDto?> GetGames();
        Task<ResponseDto?> GetNews(Guid gameId, int? pageNumber = null);
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
    }
}
