using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using PaymentService_API.Helpers;

namespace PaymentService_API.Entities
{
    public class Payment
    {
        public Payment()
        {
            Email = string.Empty;
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
        public string? StripeUrl { get; set; }
        public string? StripeId { get; set; }
        [Required]
        public PaymentStatus PaymentStatus { get; set; }
    }
}