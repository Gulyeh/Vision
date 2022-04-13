using System.ComponentModel.DataAnnotations;

namespace Identity_API.Helpers
{
    public class ConfirmEmailQuery
    {
        public ConfirmEmailQuery()
        {
            this.token = string.Empty;
        }

        [Required]
        public Guid userId { get; set; }
        [Required]
        public string token { get; set; }
    }
}