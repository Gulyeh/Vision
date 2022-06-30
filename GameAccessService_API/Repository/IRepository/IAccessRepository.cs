using GameAccessService_API.Dtos;
using GameAccessService_API.Messages;

namespace GameAccessService_API.Repository.IRepository
{
    public interface IAccessRepository
    {
        Task<(bool, BanModelDto?)> CheckUserAccess(Guid gameId, Guid userId);
        Task<ResponseDto> BanUserAccess(UserAccessDto data);
        Task<ResponseDto> UnbanUserAccess(Guid userId, Guid gameId);
        Task<bool> CheckUserHasGame(Guid gameId, Guid userId);
        Task<bool> CheckUserHasProduct(Guid productId, Guid gameId, Guid userId);
        Task<bool> AddProductOrGame(Guid userId, Guid gameId, Guid productId);
        Task RemoveGameAndProducts(DeleteGameDto data);
        Task RemoveProductAccess(Guid productId);
        Task<bool> CheckUserIsBanned(Guid userId, Guid gameId);
    }
}