using UsersService_API.Helpers;

namespace UsersService_API.Dtos
{
    public class GetUserDto
    {
        public GetUserDto()
        {
            PhotoUrl = string.Empty;
            Username = string.Empty;
            Description = string.Empty;
        }

        public Guid UserId { get; set; }
        public string PhotoUrl { get; set; }
        public string Username { get; set; }
        public Status Status { get; set; }
        public string Description { get; set; }
    }
}