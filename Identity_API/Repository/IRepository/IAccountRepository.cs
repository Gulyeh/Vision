using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity_API.Dtos;

namespace Identity_API.Repository.IRepository
{
    public interface IAccountRepository
    {
        Task<ResponseDto> Register(RegisterDto registerData, string baseUri);
        Task<ResponseDto> Login(LoginDto loginData);
        Task<ResponseDto> SingOut();
        Task<ResponseDto> ConfirmEmail(string userId, string token);
        Task<ResponseDto> BanUser(BannedUsersDto data);
        Task<ResponseDto> UnbanUser(string userId);
    }
}