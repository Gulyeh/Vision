using MessageService_API.Dtos;

namespace MessageService_API.Repository.IRepository
{
    public interface IMessageRepository
    {
        Task<(MessageDto?, bool)> SendMessage(AddMessageDto message, string access_token);
        Task<bool> DeleteMessage(DeleteMessageDto message, bool IsDelete = false);
        Task<(bool, bool)> EditMessage(EditMessageDto message);
        Task<(IEnumerable<MessageDto>, int)> GetMessages(Guid chatId, Guid userId, int pageNumber = 1);
        Task<MessageDto> GetMessage(Guid chatId, Guid messageId);
        Task SendUserMessageNotification(Guid receiverId, Guid senderId, string Access_Token);
        Task<ICollection<HasUnreadMessagesDto>> CheckUnreadMessages(ICollection<Guid> FriendsList, Guid UserId);
    }
}