namespace UsersService_API.Helpers
{
    public class OnlineUsersData
    {
        public OnlineUsersData(Guid userId, string connectionId)
        {
            UserId = userId;
            this.connectionId = connectionId;
        }

        public Guid UserId { get; private set; }
        public string connectionId { get; private set; }
    }
}