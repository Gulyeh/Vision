using System.ComponentModel.DataAnnotations;

namespace UsersService_API.Helpers
{
    public class BaseUserData
    {
        public BaseUserData()
        {
            PhotoUrl = string.Empty;
            Nickname = string.Empty;
        }

        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string PhotoUrl { get; set; }
        [Required]
        public Status Status { get; set; }
        [Required]
        public string Nickname { get; set; }
    }
}