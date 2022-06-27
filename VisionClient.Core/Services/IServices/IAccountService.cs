using VisionClient.Core.Dtos;
using VisionClient.Core.Models.Account;

namespace VisionClient.Core.Services.IServices
{
    public interface IAccountService
    {
        Task<ResponseDto> Login(LoginModel login);
        Task<ResponseDto> Register(RegisterModel register);
        Task<ResponseDto> ToggleTFA(string code);
        Task<ResponseDto> Get2FACode();
        Task<ResponseDto> RequestResetPassword(string Email);
        Task<ResponseDto> ResendEmailConfirmation(string Email);
        Task<ResponseDto> ChangePassword(ChangePasswordDto changePasswordData);
        Task<ResponseDto> GetServerData(Guid sessionId);
        Task<ResponseDto?> ChangeUserRole(Guid userId, string roleName);
        Task<ResponseDto?> GetRoles();
    }
}
