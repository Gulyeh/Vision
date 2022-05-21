using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Builders;
using VisionClient.Core.Dtos;
using VisionClient.Core.Enums;
using VisionClient.Core.Models.Account;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Core.Services.IServices;

namespace VisionClient.Core.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IAccountService accountService;

        public AccountRepository(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        public async Task<LoginResponse> LoginUser(string email, string password, string? AuthCode = null)
        {
            var loginResponse = new LoginResponseBuilder();

            var builder = new LoginModelBuilder();
            builder.SetEmail(email);
            builder.SetPassword(password);
            builder.SetAuthCode(AuthCode);
            var login = builder.Build();

            ResponseDto response = await accountService.Login(login);

            if (response.isSuccess)
            {
                var stringData = response.Response.ToString();
                if (!string.IsNullOrEmpty(stringData))
                {
                    UserDto? userData = JsonConvert.DeserializeObject<UserDto>(stringData);
                    if (userData is not null)
                    {
                        StaticData.Access_Token = userData.Token;
                        StaticData.UserData.EmailAddress = userData.Email;
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
            };

            loginResponse.SetData(response.Response);
            return loginResponse.Build();
        }

        public async Task<(bool, string?)> RegisterUser(string email, string password, string repeatpassword)
        {
            var registerBuilder = new RegisterModelBuilder();
            registerBuilder.SetEmail(email);
            registerBuilder.SetPassword(password);
            registerBuilder.SetRepeatPassword(repeatpassword);
            var registerData = registerBuilder.Build();

            var response = await accountService.Register(registerData);
            return ValidateStringResponse(response);
        }

        public async Task<(bool, string?)> RequestPasswordReset(string email)
        {
            var response = await accountService.RequestResetPassword(email);
            return ValidateStringResponse(response);
        }

        private (bool, string?) ValidateStringResponse(ResponseDto response)
        {
            if (response is null) return (false, "Something went wrong");

            var stringData = response.Response.ToString()?.Replace("[", "").Replace("]", "");
            string? json = string.Empty;
            if (stringData is not null) json = JsonConvert.DeserializeObject<string>(stringData);

            return (response.isSuccess, json);
        }
    }
}
