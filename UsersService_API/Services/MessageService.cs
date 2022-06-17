using UsersService_API.Dtos;
using UsersService_API.Helpers;
using UsersService_API.Services.IServices;

namespace UsersService_API.Services
{
    public class MessageService : BaseHttpService, IMessageService
    {
        private readonly string MessageServiceUrl;
        public MessageService(IHttpClientFactory httpClientFactory, ILogger<BaseHttpService> logger, IConfiguration config) : base(httpClientFactory, logger)
        {
            MessageServiceUrl = config["MessageServiceUrl"];
        }

        public async Task<ResponseDto?> CheckUnreadMessages(string access_token, ICollection<Guid> FriendsList)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                apiType = APIType.GET,
                ApiUrl = $"{MessageServiceUrl}/api/Message/FriendsUnreadMessages",
                Access_Token = access_token,
                Data = FriendsList
            });

            if (response is not null) return response;
            return null;
        }
    }
}