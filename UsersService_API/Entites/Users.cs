using System.ComponentModel.DataAnnotations;
using UsersService_API.Helpers;

namespace UsersService_API.Entites
{
    public class Users
    {
        public Users()
        {
            PhotoUrl = "default";
            Nickname = string.Empty;
            PhotoId = string.Empty;
        }

        [Key]
        public int Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string Nickname { get; set; }
        [Required]
        public string PhotoUrl { get; set; }
        [Required]
        public string PhotoId { get; set; }
        [Required]
        public Status Status { get; set; }
        [Required]
        public Status LastOnlineStatus { get; set; }
        public string? Description { get; set; }
    }
}