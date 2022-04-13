using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SMTPService_API.Entites
{
    public class EmailLogs
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [EmailAddress]
        [NotNull]
        public string? Email { get; set; }
        [Required]
        [NotNull]
        public string? Log { get; set; }
        [Required]
        public DateTime EmailSent { get; set; } = DateTime.Now;
    }
}