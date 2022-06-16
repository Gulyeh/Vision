namespace UsersService_API.Helpers
{
    public class OnlineUsersData
    {
        public OnlineUsersData(Guid userId, string connectionId, HubTypes hubType)
        {
            UserId = userId;
            this.connectionId = connectionId;
            HubType = hubType;
        }

        public Guid UserId { get; private set; }
        public HubTypes HubType { get; private set; }
        public string connectionId { get; private set; }
    }
}