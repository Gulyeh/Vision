using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutoMapper.Configuration.Annotations;

namespace GameAccessService_API.Dtos
{
    public class UserAccessDto
    {
        public UserAccessDto()
        {
            UserId = string.Empty;
            GameId = string.Empty;
            Reason = string.Empty;
            BlockedBy = string.Empty;
        }

        [Required]
        public string UserId { get; set; }
        [Required]
        public string GameId { get; set; }
        public string? Reason { get; set; }
        [Required]
        public string BlockedBy { get; set; }
        [JsonIgnore]
        public DateTime BlockDate { get; set; } = DateTime.Now;
        [Required]
        public DateTime ExpireDate { get; set; } 
    }
}