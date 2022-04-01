using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameAccessService_API.Dtos
{
    public class AccessDataDto
    {
        public AccessDataDto()
        {
            this.GameId = string.Empty;
            this.UserId = string.Empty;
        }

        [Required]
        public string GameId { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}