using VisionClient.Core.Dtos;
using VisionClient.Core.Enums;
using VisionClient.Core.Helpers;
using VisionClient.Core.Models.Account;
using VisionClient.Core.Services.IServices;

namespace VisionClient.Core.Services
{
    public class AccountService : HttpService, IAccountService
    {
        public AccountService(IHttpClientFactory httpClientFactory, IStaticData staticData) : base(httpClientFactory, staticData)
        {
        }

        public async Task<ResponseDto> ToggleTFA(string code)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.POST,
                ApiUrl = $"{ConnectionData.GatewayUrl}/account/Toggle2FA?code={code}",
            });

            if (response is not null) return response;
            return new ResponseDto();
        }

        public async Task<ResponseDto> Get2FACode()
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.GET,
                ApiUrl = $"{ConnectionData.GatewayUrl}/account/Generate2FA",
            });

            if (response is not null) return response;
            return new ResponseDto();
        }

        public async Task<ResponseDto> ChangePassword(ChangePasswordDto changePasswordData)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.POST,
                ApiUrl = $"{ConnectionData.GatewayUrl}/account/ChangePassword",
                Data = changePasswordData
            });

            if (response is not null) return response;
            return new ResponseDto();
        }

        public async Task<ResponseDto> Login(LoginModel login)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.POST,
                ApiUrl = $"{ConnectionData.GatewayUrl}/account/login",
                Data = login
            });

            if (response is not null) return response;
            return new ResponseDto();
        }

        public async Task<ResponseDto> Register(RegisterModel register)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.PUT,
                ApiUrl = $"{ConnectionData.GatewayUrl}/account/register",
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
                ApiUrl = $"{ConnectionData.GatewayUrl}/account/RequestResetPassword?Email={Email}"
            });

            if (response is not null) return response;
            return new ResponseDto();
        }

        public async Task<ResponseDto> GetServerData(Guid sessionId)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.GET,
                ApiUrl = $"{ConnectionData.GatewayUrl}/access/GetServerData?sessionToken={sessionId}"
            });

            if (response is not null) return response;
            return new ResponseDto();
        }

        public async Task<ResponseDto> ResendEmailConfirmation(string Email)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.GET,
                ApiUrl = $"{ConnectionData.GatewayUrl}/account/ResendConfirmationEmail?Email={Email}"
            });

            if (response is not null) return response;
            return new ResponseDto();
        }

        public async Task<ResponseDto?> ChangeUserRole(Guid userId, string roleName)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.POST,
                ApiUrl = $"{ConnectionData.GatewayUrl}/access/ChangeUserRole?userId={userId}&role={roleName}"
            });

            return response;
        }

        public async Task<ResponseDto?> GetRoles()
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.GET,
                ApiUrl = $"{ConnectionData.GatewayUrl}/access/GetRoles"
            });

            return response;
        }
    }
}
