using System.Runtime.Serialization;

namespace Identity_API.Statics
{
    public class StaticData
    {
        public const string UserRole = "User";
        public const string AdminRole = "Administrator";
        public const string ModeratorRole = "Moderator";

        public enum RoleValue
        {
            User = 0,
            Moderator = 1,
            Administrator = 2
        }      
    }
}