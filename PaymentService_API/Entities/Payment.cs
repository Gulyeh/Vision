using PaymentService_API.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentService_API.Entities
{
    public class Payment
    {
        public Payment()
        {
            Email = string.Empty;
            Provider = string.Empty;
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid OrderId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string Email { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        [Required]
        public decimal TotalPrice { get; set; }
        public string? PaymentUrl { get; set; }
        public string? PaymentId { get; set; }
        [Required]
        public PaymentStatus PaymentStatus { get; set; }
        [Required]
        public string Provider { get; set; }
    }
}