using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionClient.Core.Dtos
{
    public class UserDto
    {
        public UserDto()
        {
            Token = string.Empty;
            Email = string.Empty;
        }

        public string Email { get; set; }
        public string Token { get; set; }
    }
}
