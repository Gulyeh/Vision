using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameAccessService_API.Dtos
{
    public class AccessDataDto
    {
        [Required]
        public Guid GameId { get; set; }
        [Required]
        public Guid UserId { get; set; }
    }
}