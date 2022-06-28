using Microsoft.AspNetCore.Http;
using VisionClient.Core.Dtos;
using VisionClient.Core.Models.Account;

namespace VisionClient.Core.Repository.IRepository
{
    public interface IAccountRepository
    {
        Task<LoginResponse> LoginUser(string email, string password, string? AuthCode = null);
        Task<(bool, string)> RegisterUser(string email, string password, string repeatpassword);
        Task<(bool, string)> RequestPasswordReset(string email);
        Task<(bool, string)> ResendEmailConfirmation(string email);
        Task<(bool, string)> ChangePassword(string currentPassword, string newPassword, string repeatPassword);
        Task<(bool, string)> Toggle2FA(string code);
        Task<(bool, object?)> Generate2FA();
        Task<bool> GetServerData();
        Task<List<string>> GetRoles();
        Task<string> ChangeUserRole(Guid userId, string roleName);
        Task<(bool, string)> UnbanUser(Guid userId);
        Task<(bool, string)> BanUser(BanModelDto data);
        Task<(string, int)> DeleteAccount(LoginModel data);
    }
}
