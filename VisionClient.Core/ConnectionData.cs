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
        public static string UpdateData = "https://gist.github.com/Gulyeh/be47e02d0950aed152385f5b8b3fe5ce#file-updateversion-xml";
    }

}
