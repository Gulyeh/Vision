using System.ComponentModel.DataAnnotations;

namespace Identity_API.Helpers
{
    public class BasicUserData
    {
        public BasicUserData()
        {
            Password = string.Empty;
            Email = string.Empty;
        }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(maximumLength: 15, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters and maximum 15 characters long")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Wrong email format")]
        public string Email { get; set; }
    }
}