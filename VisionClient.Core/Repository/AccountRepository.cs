using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using VisionClient.Core.Builders;
using VisionClient.Core.Dtos;
using VisionClient.Core.Enums;
using VisionClient.Core.Helpers;
using VisionClient.Core.Models;
using VisionClient.Core.Models.Account;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Core.Services.IServices;

namespace VisionClient.Core.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IAccountService accountService;
        private readonly IStaticData StaticData;

        public AccountRepository(IAccountService accountService, IStaticData staticData)
        {
            this.accountService = accountService;
            StaticData = staticData;
        }

        public async Task<bool> GetServerData()
        {
            var response = await accountService.GetServerData(StaticData.SessionId);
            if (response is null || !response.isSuccess)
            {
                StaticData.UserData = new();
                StaticData.SessionId = Guid.Empty;
                return false;
            }

            var stringData = response.Response.ToString();
            if (!string.IsNullOrEmpty(stringData))
            {
                ServerDataModel? serverData = JsonConvert.DeserializeObject<ServerDataModel>(stringData);
                if (serverData is not null && !serverData.IsAnyNullOrEmpty()) return true;
            }

            StaticData.UserData = new();
            StaticData.SessionId = Guid.Empty;
            return false;
        }

        public async Task<(bool, string)> ChangePassword(string currentPassword, string newPassword, string repeatPassword)
        {
            var changePasswordBuilder = new ChangePasswordBuilder();
            changePasswordBuilder.SetNewPassword(newPassword);
            changePasswordBuilder.SetRepeatPassword(repeatPassword);
            changePasswordBuilder.SetCurrentPassword(currentPassword);

            ResponseDto response = await accountService.ChangePassword(changePasswordBuilder.Build());
            return ValidateStringResponse(response);
        }

        public async Task<(bool, object?)> Generate2FA()
        {
            ResponseDto response = await accountService.Get2FACode();
            if (response is null) return (false, "Something went wrong");

            if (!response.isSuccess) return ValidateStringResponse(response);

            var stringData = response.Response.ToString();

            TFADataModel? json = new();
            if (stringData is not null) json = JsonConvert.DeserializeObject<TFADataModel>(stringData);

            return (response.isSuccess, json);
        }

        public async Task<LoginResponse> LoginUser(string email, string password, string? AuthCode = null)
        {
            var loginResponse = new LoginResponseBuilder();

            var builder = new LoginModelBuilder();
            builder.SetEmail(email);
            builder.SetPassword(password);
            builder.SetAuthCode(AuthCode);

            ResponseDto response = await accountService.Login(builder.Build());

            if (response.isSuccess)
            {
                var stringData = response.Response.ToString();
                if (!string.IsNullOrEmpty(stringData))
                {
                    UserDto? userData = JsonConvert.DeserializeObject<UserDto>(stringData);
                    if (userData is not null)
                    {
                        StaticData.UserData.Email = userData.Email;
                        StaticData.UserData.Access_Token = userData.Token;
                        StaticData.SessionId = userData.SessionId;
                        loginResponse.SetType(LoginResponseTypes.Success);
                    }
                }
                return loginResponse.Build();
            }

            switch (response.Status)
            {
                case StatusCodes.Status200OK:
                    loginResponse.SetType(LoginResponseTypes.TwoFactorAuth);
                    break;
                case StatusCodes.Status400BadRequest:
                    loginResponse.SetType(LoginResponseTypes.WrongAuthCode);
                    break;
                case StatusCodes.Status401Unauthorized:
                    loginResponse.SetType(LoginResponseTypes.WrongCredentials);
                    break;
                case StatusCodes.Status403Forbidden:
                    loginResponse.SetType(LoginResponseTypes.UserBanned);
                    break;
                default:
                    loginResponse.SetType(LoginResponseTypes.WrongCredentials);
                    break;
            };

            loginResponse.SetData(response.Response);
            return loginResponse.Build();
        }

        public async Task<(bool, string)> RegisterUser(string email, string password, string repeatpassword)
        {
            var registerBuilder = new RegisterModelBuilder();
            registerBuilder.SetEmail(email);
            registerBuilder.SetPassword(password);
            registerBuilder.SetRepeatPassword(repeatpassword);

            var response = await accountService.Register(registerBuilder.Build());
            return ValidateStringResponse(response);
        }

        public async Task<(bool, string)> RequestPasswordReset(string email)
        {
            var response = await accountService.RequestResetPassword(email);
            return ValidateStringResponse(response);
        }

        public async Task<(bool, string)> Toggle2FA(string code)
        {
            var response = await accountService.ToggleTFA(code);
            return ValidateStringResponse(response);
        }

        private static (bool, string) ValidateStringResponse(ResponseDto response)
        {
            if (response is null) return (false, "Something went wrong");
            return (response.isSuccess, ResponseToJsonHelper.GetJson(response));
        }

        public async Task<(bool, string)> ResendEmailConfirmation(string email)
        {
            var response = await accountService.ResendEmailConfirmation(email);
            return ValidateStringResponse(response);
        }

        public async Task<List<string>> GetRoles()
        {
            var response = await accountService.GetRoles();
            if (response is null) throw new Exception();
            return ResponseToJsonHelper.GetJson<List<string>>(response);
        }

        public async Task<string> ChangeUserRole(Guid userId, string roleName)
        {
            var response = await accountService.ChangeUserRole(userId, roleName);
            if (response is null) throw new Exception();
            return ResponseToJsonHelper.GetJson(response);
        }

        public async Task<(bool, string)> UnbanUser(Guid userId)
        {
            var response = await accountService.UnbanUser(userId);
            if (response is null) throw new Exception();
            return (response.isSuccess, ResponseToJsonHelper.GetJson(response));
        }

        public async Task<(bool, string)> BanUser(BanModelDto data)
        {
            var response = await accountService.BanUser(data);
            if (response is null) throw new Exception();
            return (response.isSuccess, ResponseToJsonHelper.GetJson(response));
        }
    }
}
