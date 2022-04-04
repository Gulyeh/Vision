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
            Reason = string.Empty;
            BlockedBy = string.Empty;
        }

        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid GameId { get; set; }
        public string? Reason { get; set; }
        [Required]
        public string BlockedBy { get; set; }
        [JsonIgnore]
        public DateTime BlockDate { get; set; } = DateTime.Now;
        [Required]
        public DateTime ExpireDate { get; set; } 
    }
}