using Identity_API.Dtos;

namespace Identity_API.Repository.IRepository
{
    public interface IAccountRepository
    {
        Task<ResponseDto> ResendEmailConfirmation(string email);
        Task<ResponseDto> Register(RegisterDto registerData);
        Task<ResponseDto> Login(LoginDto loginData);
        Task<ResponseDto> ConfirmEmail(Guid userId, string token);
        Task<ResponseDto> RequestResetPassword(string Email);
        Task<ResponseDto> ResetPassword(ResetPasswordDto data);
        Task<ResponseDto> ChangePassword(PasswordDataDto data);
        Task<ResponseDto> Toggle2FA(Guid userId, string code);
        Task<ResponseDto> Generate2FA(Guid userId);
    }
}