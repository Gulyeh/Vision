using System.ComponentModel.DataAnnotations;

namespace OrderService_API.Dtos
{
    public class OrderDto
    {
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