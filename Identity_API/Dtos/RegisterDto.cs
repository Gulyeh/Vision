using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Identity_API.Entites;

namespace Identity_API.Dtos
{
    public class RegisterDto : BasicUserData
    {
        public RegisterDto(string password, string email, string confirmpassword) : base(password, email)
        {
            ConfirmPassword = confirmpassword;
        }
        [Required(ErrorMessage = "Confirm Password required")]
        [Compare("Password", ErrorMessage = "Password doesn't match.")]
        public string ConfirmPassword { get; set; }
    }
}