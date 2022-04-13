using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Identity_API.Dtos
{
    public class BannedUsersDto
    {
        public BannedUsersDto()
        {
            Reason = string.Empty;
        }
        [Required]
        public Guid UserId { get; set; }
        public string? Reason { get; set; }
        [JsonIgnore]
        public DateTime BanDate { get; set; } = DateTime.Now;
        [Required]
        public DateTime BanExpires { get; set; }
    }
}