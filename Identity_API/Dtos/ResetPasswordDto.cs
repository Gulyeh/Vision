using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Identity_API.Dtos
{
    public class ResetPasswordDto
    {
        public ResetPasswordDto()
        {
            Token = string.Empty;
            NewPassword = string.Empty;
            ConfirmNewPassword = string.Empty;
        }

        [Required]
        public Guid userId { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword", ErrorMessage = "Password doesn't match.")]
        public string ConfirmNewPassword { get; set; }
    }
}