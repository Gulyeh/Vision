using VisionClient.Core.Dtos;
using VisionClient.Core.Enums;
using VisionClient.Core.Helpers;
using VisionClient.Core.Services.IServices;

namespace VisionClient.Core.Services
{
    public class UsersService : HttpService, IUsersService
    {
        public UsersService(IHttpClientFactory httpClientFactory, IStaticData staticData) : base(httpClientFactory, staticData)
        {
        }

        public async Task<ResponseDto?> FindUser(string contains)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.GET,
                ApiUrl = $"{ConnectionData.GatewayUrl}/Users/FindUser?containsString={contains}",
            });

            return response;
        }

        public async Task<ResponseDto?> ChangePhoto(string image)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.POST,
                ApiUrl = $"{ConnectionData.GatewayUrl}/Users/ChangePhoto",
                Data = image
            });

            return response;
        }

        public async Task<ResponseDto?> GetDetailedUsers(string containsString)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.GET,
                ApiUrl = $"{ConnectionData.GatewayUrl}/Users/FindDetailedUser?containsString={containsString}",
            });

            return response;
        }

        public async Task<ResponseDto?> ChangeCurrency(Guid userId, int Amount)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                ApiType = APIType.PUT,
                ApiUrl = $"{ConnectionData.GatewayUrl}/Users/ChangeCurrency?userId={userId}&amount={Amount}",
            });

            return response;
        }
    }
}
