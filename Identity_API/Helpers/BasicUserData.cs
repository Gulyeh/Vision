using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Identity_API.Helpers
{
    public class BasicUserData
    {
        public BasicUserData(string password, string email)
        {
            Password = password;
            Email = email;
        }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(maximumLength: 15, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters and maximum 15 characters long")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Wrong email format")]
        public string Email { get; set; }
    }
}