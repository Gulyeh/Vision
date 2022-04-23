namespace Identity_API.Dtos
{
    public class UserDto
    {
        public UserDto()
        {
            Username = string.Empty;
            Token = string.Empty;
            PhotoUrl = string.Empty;
            Description = string.Empty;
            Email = string.Empty;
        }

        public string Email { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public string PhotoUrl { get; set; }
        public string Description { get; set; }
    }
}