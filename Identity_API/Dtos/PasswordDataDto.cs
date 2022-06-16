using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Identity_API.Dtos
{
    public class PasswordDataDto
    {
        public PasswordDataDto()
        {
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            RepeatPassword = string.Empty;
        }

        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword", ErrorMessage = "Password does not match")]
        public string RepeatPassword { get; set; }
        [JsonIgnore]
        public Guid userId { get; set; }
    }
}