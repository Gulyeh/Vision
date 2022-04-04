using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Identity_API.Helpers;

namespace Identity_API.Dtos
{
    public class RegisterDto : BasicUserData
    {
        public RegisterDto()
        {
            ConfirmPassword = string.Empty;
        }
        [Required(ErrorMessage = "Confirm Password required")]
        [Compare("Password", ErrorMessage = "Password doesn't match.")]
        public string ConfirmPassword { get; set; }
    }
}