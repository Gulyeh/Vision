namespace MessageService_API.Services.IServices
{
    public interface IUsersService
    {
        Task<T?> CheckUserExists<T>(Guid userId, string Access_Token);
        Task<T?> SendUserMessageNotification<T>(Guid userId, Guid chatId, string Access_Token);
    }
}