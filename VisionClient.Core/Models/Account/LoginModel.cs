using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionClient.Core.Models.Account
{
    public class LoginModel
    {
        public LoginModel()
        {
            Email = string.Empty;
            Password = string.Empty;
        }

        public string Email { get; set; }
        public string Password { get; set; } 
        public string? AuthCode { get; set; }
    }
}
