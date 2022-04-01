using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameAccessService_API.Entites
{
    public class UserAccess
    {
        public UserAccess()
        {
            UserId = string.Empty;
            GameId = string.Empty;
            Reason = string.Empty;
            BlockedBy = string.Empty;
        }

        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string GameId { get; set; }
        public string? Reason { get; set; }
        [Required]
        public string BlockedBy { get; set; }
        [Required]
        public DateTime BlockDate { get; set; }
        [Required]
        public DateTime ExpireDate { get; set; }   
    }
}