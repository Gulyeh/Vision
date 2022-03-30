using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace SMTPService_API.Entities
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