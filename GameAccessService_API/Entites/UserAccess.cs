
using GameAccessService_API.Helpers;
using System.ComponentModel.DataAnnotations;

namespace GameAccessService_API.Entites
{
    public class UserAccess : BaseUser
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public Guid GameId { get; set; }
        public string Reason { get; set; } = string.Empty;
        [Required]
        public DateTime BanDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
        [Required]
        public DateTime BanExpires { get; set; }
    }
}