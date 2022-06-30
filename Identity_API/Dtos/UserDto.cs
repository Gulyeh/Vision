namespace Identity_API.Dtos
{
    public class UserDto
    {
        public UserDto()
        {
            Token = string.Empty;
            Email = string.Empty;
        }

        public string Email { get; set; }
        public string Token { get; set; }
        public Guid SessionId { get; set; }
    }
}