using System.ComponentModel.DataAnnotations;
using UsersService_API.Statics;

namespace UsersService_API.Helpers
{
    public class BaseUserData
    {
        public BaseUserData()
        {
            PhotoUrl = string.Empty;
            Username = string.Empty;
            PhotoUrl = StaticData.DefaultPhoto;
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