using VisionClient.Core.Enums;
using VisionClient.Core.Models;

namespace VisionClient.Core
{
    public static class StaticData
    {
        public static string Access_Token = string.Empty;
        public static UserModel UserData = new UserModel();

        public static readonly string BaseIP = "localhost";
        public static readonly string IdentityServerUrl = $"https://{BaseIP}:7271/api/";
    }
}
