namespace Identity_API.Dtos
{
    public class UserDto
    {
        public UserDto(string username, string token, string photoUrl, string description, string email)
        {
            Username = username;
            Token = token;
            PhotoUrl = photoUrl;
            Description = description;
            Email = email;
        }

        public string Email { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public string PhotoUrl { get; set; }
        public string Description { get; set; }
    }
}