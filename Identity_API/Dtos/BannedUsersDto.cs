using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Identity_API.Dtos
{
    public class BannedUsersDto
    {
        public BannedUsersDto()
        {
            UserId = string.Empty;
            Reason = string.Empty;
        }
        [Required]
        public string UserId { get; set; }
        public string? Reason { get; set; }
        [JsonIgnore]
        public DateTime BanDate { get; set; } = DateTime.Now;
        [Required]
        public DateTime BanExpires { get; set; }
    }
}