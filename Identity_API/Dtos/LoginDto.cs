using Identity_API.Helpers;

namespace Identity_API.Dtos
{
    public class LoginDto : BasicUserData
    {
        public string? AuthCode { get; set; }
    }
}