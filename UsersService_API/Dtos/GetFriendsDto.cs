using UsersService_API.Helpers;

namespace UsersService_API.Dtos
{
    public class GetFriendsDto : BaseUserData
    {
        public bool HasUnreadMessages { get; set; }
        public bool IsBlocked { get; set; }
    }
}