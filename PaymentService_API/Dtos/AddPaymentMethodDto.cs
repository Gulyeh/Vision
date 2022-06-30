using System.ComponentModel.DataAnnotations;

namespace PaymentService_API.Dtos
{
    public class AddPaymentMethodDto
    {
        public AddPaymentMethodDto()
        {
            Photo = string.Empty;
            Provider = string.Empty;
        }

        [Required]
        public string Photo { get; set; }
        [Required]
        public bool IsAvailable { get; set; }
        [Required]
        public string Provider { get; set; }
    }
}