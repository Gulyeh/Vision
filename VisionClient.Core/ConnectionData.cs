namespace VisionClient.Core
{
    public sealed class ConnectionData
    {
        public static readonly string BaseIP = "localhost";
        public static readonly string Port = "7271";
        public static readonly string GatewayUrl = $"https://{BaseIP}:{Port}/api";
        public static string UsersHub = string.Empty;
        public static string FriendsHub = string.Empty;
        public static string MessageHub = string.Empty;
        public static string OrderHub = string.Empty;
    }

}
