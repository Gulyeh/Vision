namespace VisionClient.Core.Models.Account
{
    public class ServerDataModel
    {
        public string UsersHub
        {
            get { return ConnectionData.UsersHub; }
            set { ConnectionData.UsersHub = value; }
        }

        public string FriendsHub
        {
            get { return ConnectionData.FriendsHub; }
            set { ConnectionData.FriendsHub = value; }
        }

        public string MessageHub
        {
            get { return ConnectionData.MessageHub; }
            set { ConnectionData.MessageHub = value; }
        }
        public string OrderHub
        {
            get { return ConnectionData.OrderHub; }
            set { ConnectionData.OrderHub = value; }
        }

        public bool IsAnyNullOrEmpty()
        {
            return string.IsNullOrWhiteSpace(UsersHub) || string.IsNullOrWhiteSpace(FriendsHub) || string.IsNullOrWhiteSpace(MessageHub) || string.IsNullOrWhiteSpace(OrderHub);
        }
    }
}
