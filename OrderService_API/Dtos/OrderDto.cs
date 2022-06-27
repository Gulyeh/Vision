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

        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public OrderType OrderType { get; set; }
        public Guid UserId { get; set; }
        public Guid? PaymentId { get; set; }
        public bool Paid { get; set; }
        public DateTime OrderDate { get; set; }
        public string Title { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string CouponCode { get; set; }
        public Guid? GameId { get; set; }

    }
}