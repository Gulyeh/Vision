using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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