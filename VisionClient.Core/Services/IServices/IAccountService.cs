using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Dtos;
using VisionClient.Core.Models;
using VisionClient.Core.Models.Account;

namespace VisionClient.Core.Services.IServices
{
    public interface IAccountService
    {
        Task<ResponseDto> Login(LoginModel login);
        void Logout();
        Task<ResponseDto> Register(RegisterModel register);
        Task<ResponseDto> ActivateTFA();
        Task<ResponseDto> RequestResetPassword(string Email);
    }
}
