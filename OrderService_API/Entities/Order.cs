using OrderService_API.Helpers;
using System.ComponentModel.DataAnnotations;

namespace OrderService_API.Entities
{
    public class Order
    {
        public Order()
        {
            Title = string.Empty;
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
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
        public DateTime OrderDate { get; private init; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
        public DateTime? PaymentDate { get; set; }
        public string? CuponCode { get; set; }

    }
}