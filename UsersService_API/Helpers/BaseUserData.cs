using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UsersService_API.Helpers
{
    public class BaseUserData
    {
        public BaseUserData()
        {
            PhotoUrl = string.Empty;
            Nickname = string.Empty;
        }

        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string PhotoUrl { get; set; }
        [Required]
        public Status Status { get; set; }
        [Required]
        public string Nickname { get; set; }
    }
}