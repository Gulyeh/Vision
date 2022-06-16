using PaymentService_API.Helpers;
using System.ComponentModel.DataAnnotations;

namespace PaymentService_API.Entities
{
    public class PaymentMethods
    {
        public PaymentMethods()
        {
            PhotoUrl = string.Empty;
            PhotoId = string.Empty;
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        public string PhotoUrl { get; set; }
        [Required]
        public string PhotoId { get; set; }
        [Required]
        public bool IsAvailable { get; set; }
        [Required]
        public PaymentProvider Provider { get; set; }
    }
}