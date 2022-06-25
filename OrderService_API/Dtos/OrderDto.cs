using OrderService_API.Helpers;
using System.ComponentModel.DataAnnotations;

namespace OrderService_API.Dtos
{
    public class OrderDto
    {
        public OrderDto()
        {
            Title = string.Empty;
            CouponCode = string.Empty;
        }

        [Required]
        public Guid Id { get; set; }
        [Required]
        public Guid ProductId { get; set; }
        [Required]
        public OrderType OrderType { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public Guid? PaymentId { get; set; }
        [Required]
        public bool Paid { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public string Title { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string CouponCode { get; set; }
        public Guid? GameId { get; set; }

    }
}