namespace VisionClient.Core.Dtos
{
    public class UserDto
    {
        public UserDto()
        {
            Email = string.Empty;
            Token = string.Empty;
        }

        public string Email { get; set; }

        public string Token { get; set; }

        public Guid SessionId { get; set; }
    }
}
