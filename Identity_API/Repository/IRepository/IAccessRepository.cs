using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity_API.Dtos;

namespace Identity_API.Repository.IRepository
{
    public interface IAccessRepository
    {
        Task<ResponseDto> BanUser(BannedUsersDto data);
        Task<ResponseDto> UnbanUser(string userId);
    }
}