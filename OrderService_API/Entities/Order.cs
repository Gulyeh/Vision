using System.ComponentModel.DataAnnotations;
using OrderService_API.Helpers;

namespace OrderService_API.Entities
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid ProductId { get; set; }
        [Required]
        public OrderType OrderType { get; set; }
        public Guid? GameId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public Guid? PaymentId { get; set; }
        [Required]
        public bool Paid { get; set; } = false;
        [Required]
        public DateTime OrderDate { get; private init; } = DateTime.UtcNow;
        public DateTime? PaymentDate { get; set; }
        public string? CuponCode { get; set; }

    }
}