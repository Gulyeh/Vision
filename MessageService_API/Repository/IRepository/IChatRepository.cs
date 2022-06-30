namespace MessageService_API.Repository.IRepository
{
    public interface IChatRepository
    {
        Task<Guid> ChatExists(Guid user1, Guid user2);
        Task<Guid> CreateChat(Guid receiverId, Guid senderId);
        Task DeleteChat(Guid user1, Guid user2);
    }
}