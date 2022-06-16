using System.ComponentModel.DataAnnotations;

namespace UsersService_API.Helpers
{
    public class BaseUserData
    {
        public BaseUserData()
        {
            PhotoUrl = string.Empty;
            Username = string.Empty;
            PhotoUrl = "https://res.cloudinary.com/dhj8btqwp/image/upload/v1653243593/default_ya5gro.png";
            Username = "VisionUser";
            Status = Status.Offline;
            Description = string.Empty;
        }

        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string PhotoUrl { get; set; }
        [Required]
        public Status Status { get; set; }
        [Required]
        public string Username { get; set; }
        public string Description { get; set; }
    }
}