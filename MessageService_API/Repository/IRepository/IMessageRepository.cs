using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessageService_API.Dtos;

namespace MessageService_API.Repository.IRepository
{
    public interface IMessageRepository
    {
        Task<MessageDto> SendMessage(AddMessageDto message);
        Task<bool> DeleteMessage(DeleteMessageDto message);
        Task<bool> EditMessage(EditMessageDto message);
        Task<IEnumerable<MessageDto>> GetMessages(Guid chatId, Guid userId);
        Task<MessageDto> GetMessage(Guid chatId, Guid messageId);
        Task SendUserMessageNotification(Guid chatId, Guid receiverId, string Access_Token);
    }
}