namespace VisionClient.Core
{
    public sealed class ConnectionData
    {
        public static readonly string BaseIP = "vision.servegame.com";
        public static readonly string Port = "7271";
        public static readonly string GatewayUrl = $"http://{BaseIP}:{Port}/api";
        public static string UsersHub = string.Empty;
        public static string FriendsHub = string.Empty;
        public static string MessageHub = string.Empty;
        public static string OrderHub = string.Empty;
        public static string UpdateData = "https://gist.githubusercontent.com/Gulyeh/be47e02d0950aed152385f5b8b3fe5ce/raw/58f6331708970dbe14eb4b9c7ba9a211d23999b9/UpdateVersion.xml";
    }

}
