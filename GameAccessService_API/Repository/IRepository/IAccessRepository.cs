using GameAccessService_API.Dtos;

namespace GameAccessService_API.Repository.IRepository
{
    public interface IAccessRepository
    {
        Task<(bool, BanModelDto?)> CheckUserAccess(Guid gameId, Guid userId);
        Task<ResponseDto> BanUserAccess(UserAccessDto data);
        Task<ResponseDto> UnbanUserAccess(AccessDataDto data);
        Task<bool> CheckUserHasGame(Guid gameId, Guid userId);
        Task<bool> CheckUserHasProduct(Guid productId, Guid gameId, Guid userId);
        Task<bool> AddProductOrGame(Guid userId, Guid gameId, Guid productId);
    }
}