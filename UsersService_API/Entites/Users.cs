using System.ComponentModel.DataAnnotations;
using UsersService_API.Helpers;

namespace UsersService_API.Entites
{
    public class Users
    {
        public Users()
        {
            PhotoUrl = "default";
            Nickname = "VisionUser";
            PhotoId = string.Empty;
            CurrencyValue = 0;
            Status = Status.Offline;
            LastOnlineStatus = Status.Offline;
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
        [Required]
        public int CurrencyValue { get; set; }
        public string? Description { get; set; }
    }
}