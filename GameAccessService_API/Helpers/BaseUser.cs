using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameAccessService_API.Helpers
{
    public class BaseUser
    {
        [Required]
        public Guid GameId { get; set; }
        [Required]
        public Guid UserId { get; set; }
    }
}