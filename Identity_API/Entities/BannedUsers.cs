using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Identity_API.Entities
{
    public class BannedUsers
    {
        public BannedUsers()
        {
            UserId = string.Empty;
            Reason = string.Empty;
        }

        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        public string? Reason { get; set; }
        [Required]
        public DateTime BanDate { get; set; }
        [Required]
        public DateTime BanExpires { get; set; }
    }
}