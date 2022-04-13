using UsersService_API.Helpers;

namespace UsersService_API.Dtos
{
    public class UserDataDto : BaseUserData
    {
        public string? Description { get; set; }
    }
}