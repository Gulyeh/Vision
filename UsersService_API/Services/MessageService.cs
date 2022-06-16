using UsersService_API.Dtos;
using UsersService_API.Helpers;
using UsersService_API.Services.IServices;

namespace UsersService_API.Services
{
    public class MessageService : BaseHttpService, IMessageService
    {
        public MessageService(IHttpClientFactory httpClientFactory, ILogger<BaseHttpService> logger) : base(httpClientFactory, logger)
        {
        }

        public async Task<ResponseDto?> CheckUnreadMessages(string access_token, ICollection<Guid> FriendsList)
        {
            var response = await SendAsync<ResponseDto>(new ApiRequest()
            {
                apiType = APIType.GET,
                ApiUrl = "https://localhost:7015/api/Message/FriendsUnreadMessages",
                Access_Token = access_token,
                Data = FriendsList
            });

            if (response is not null) return response;
            return null;
        }
    }
}