using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService_API.Entities
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid GameId { get; set; }
        public Guid? ProductId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public Guid? PaymentId { get; set; }
        [Required]
        public bool Paid { get; set; } = false;
        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public DateTime? PaymentDate { get; set; }
        public string? CuponCode { get; set; }

    }
}