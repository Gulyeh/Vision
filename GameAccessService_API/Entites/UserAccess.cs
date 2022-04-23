
using GameAccessService_API.Helpers;
using System.ComponentModel.DataAnnotations;

namespace GameAccessService_API.Entites
{
    public class UserAccess : BaseUser
    {
        public UserAccess()
        {
            Reason = string.Empty;
            BlockedBy = string.Empty;
        }

        [Key]
        public int Id { get; set; }
        public string? Reason { get; set; }
        [Required]
        public string BlockedBy { get; set; }
        [Required]
        public DateTime BlockDate { get; set; } = DateTime.UtcNow;
        [Required]
        public DateTime ExpireDate { get; set; }
    }
}