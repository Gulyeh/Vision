using System.ComponentModel.DataAnnotations;

namespace Identity_API.Entities
{
    public class BannedUsers
    {
        public BannedUsers()
        {
            Reason = string.Empty;
        }

        [Key]
        public int Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public string Reason { get; set; }
        [Required]
        public DateTime BanDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
        [Required]
        public DateTime BanExpires { get; set; }
    }
}