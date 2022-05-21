using Identity_API.Dtos;
using Identity_API.Helpers;

namespace Identity_API.Repository.IRepository
{
    public interface IAccountRepository
    {
        Task<ResponseDto> Register(RegisterDto registerData, string baseUri);
        Task<ResponseDto> Login(LoginDto loginData);
        Task<ResponseDto> ConfirmEmail(Guid userId, string token);
        Task<ResponseDto> RequestResetPassword(string Email, string baseUri);
        Task<ResponseDto> ResetPassword(ResetPasswordDto data);
    }
}