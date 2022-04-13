using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameAccessService_API.Dtos;

namespace GameAccessService_API.Repository.IRepository
{
    public interface IAccessRepository
    {
        Task<bool> CheckUserAccess(Guid gameId, Guid userId);
        Task<ResponseDto> BanUserAccess(UserAccessDto data);
        Task<ResponseDto> UnbanUserAccess(AccessDataDto data);
        Task<bool> CheckUserHasGame(Guid gameId, Guid userId);
        Task<bool> CheckUserHasProduct(Guid productId, Guid userId);
        Task<bool> AddProductOrGame(Guid userId, Guid gameId, Guid? productId = null);
    }
}