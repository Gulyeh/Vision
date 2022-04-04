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
    }
}