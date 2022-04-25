using MessageService_API.Helpers;
using MessageService_API.Services.IServices;
using MessageService_API.Static;

namespace MessageService_API.Services
{
    public class UsersService : BaseHttpService, IUsersService
    {
        public UsersService(IHttpClientFactory httpClientFactory, ILogger<BaseHttpService> logger) : base(httpClientFactory, logger)
        {
        }

        public async Task<T?> CheckUserExists<T>(Guid userId, string Access_Token)
        {
            var response = await SendAsync<T>(new ApiRequest()
            {
                apiType = APIType.GET,
                ApiUrl = $"{APIUrls.UsersService}api/users/userexists?userId={userId}",
                Access_Token = Access_Token
            });

            if (response is not null) return response;
            return default(T);
        }

        public async Task<T?> SendUserMessageNotification<T>(Guid userId, Guid chatId, string Access_Token)
        {
            var response = await SendAsync<T>(new ApiRequest()
            {
                apiType = APIType.GET,
                ApiUrl = $"{APIUrls.UsersService}api/users/MessageNotification?receiverId={userId}&chatId={chatId}",
                Access_Token = Access_Token
            });

            if (response is not null) return response;
            return default(T);
        }
    }
}