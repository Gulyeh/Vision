using MessageService_API.Dtos;
using MessageService_API.Helpers;
using MessageService_API.Services.IServices;

namespace MessageService_API.Services
{
    public class UsersService : BaseHttpService, IUsersService
    {
        private readonly string UsersServiceUrl;

        public UsersService(IHttpClientFactory httpClientFactory, ILogger<BaseHttpService> logger, IConfiguration config) : base(httpClientFactory, logger)
        {
            UsersServiceUrl = config.GetValue<string>("UsersServiceUrl");
        }

        public async Task<ResponseDto?> CheckIfUserIsBlocked(Guid userId, Guid user2Id, string Access_Token)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                apiType = APIType.GET,
                ApiUrl = $"{UsersServiceUrl}/api/Users/IsUserBlocked?userId={userId}&user2Id={user2Id}",
                Access_Token = Access_Token
            });

            if (response is not null) return response;
            return null;
        }

        public async Task<ResponseDto?> CheckUserExists(Guid userId, string Access_Token)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                apiType = APIType.GET,
                ApiUrl = $"{UsersServiceUrl}/api/Users/UserExists?userId={userId}",
                Access_Token = Access_Token
            });

            if (response is not null) return response;
            return null;
        }

        public async Task<ResponseDto?> SendUserMessageNotification(Guid receiverId, Guid senderId, string Access_Token)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                apiType = APIType.GET,
                ApiUrl = $"{UsersServiceUrl}/api/users/MessageNotification?receiverId={receiverId}&senderId={senderId}",
                Access_Token = Access_Token
            });

            if (response is not null) return response;
            return null;
        }
    }
}