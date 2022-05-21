using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Dtos;
using VisionClient.Core.Helpers;
using VisionClient.Core.Models;
using VisionClient.Core.Models.Account;
using VisionClient.Core.Services.IServices;

namespace VisionClient.Core.Services
{
    public class AccountService : HttpService, IAccountService
    {
        public AccountService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public async Task<ResponseDto> ActivateTFA()
        {
            return new ResponseDto();
        }

        public async Task<ResponseDto> Login(LoginModel login)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.POST,
                ApiUrl = $"{StaticData.IdentityServerUrl}account/login",
                Data = login
            });

            if (response is not null) return response;
            return new ResponseDto();
        }

        public void Logout()
        {
            StaticData.UserData = new UserModel();
            StaticData.Access_Token = string.Empty;     
        }

        public async Task<ResponseDto> Register(RegisterModel register)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.POST,
                ApiUrl = $"{StaticData.IdentityServerUrl}account/register",
                Data = register
            });

            if (response is not null) return response;
            return new ResponseDto();
        }

        public async Task<ResponseDto> RequestResetPassword(string Email)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.GET,
                ApiUrl = $"{StaticData.IdentityServerUrl}account/RequestResetPassword?Email={Email}"
            });

            if (response is not null) return response;
            return new ResponseDto();
        }
    }
}
