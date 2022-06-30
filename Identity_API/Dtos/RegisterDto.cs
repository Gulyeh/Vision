using Identity_API.Helpers;
using System.ComponentModel.DataAnnotations;

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