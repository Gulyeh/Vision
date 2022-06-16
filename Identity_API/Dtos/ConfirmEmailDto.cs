using System.ComponentModel.DataAnnotations;

namespace Identity_API.Dtos
{
    public class ConfirmEmailDto
    {
        public ConfirmEmailDto()
        {
            Token = string.Empty;
        }

        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string Token { get; set; }
    }
}