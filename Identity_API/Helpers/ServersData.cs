namespace Identity_API.Helpers
{
    public class ServersData
    {
        public ServersData()
        {
            UsersHub = string.Empty;
            FriendsHub = string.Empty;
            MessageHub = string.Empty;
            OrderHub = string.Empty;
        }

        public string UsersHub { get; set; }
        public string FriendsHub { get; set; }
        public string MessageHub { get; set; }
        public string OrderHub { get; set; }

        public object GetServerData()
        {
            return new { UsersHub = this.UsersHub, FriendsHub = this.FriendsHub, MessageHub = this.MessageHub, OrderHub = this.OrderHub };
        }
    }
}