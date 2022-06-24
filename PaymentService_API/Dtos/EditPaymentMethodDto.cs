using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentService_API.Dtos
{
    public class EditPaymentMethodDto
    {
        public EditPaymentMethodDto()
        {
            Photo = string.Empty;
        }

        [Required]
        public Guid Id { get; set; }
        public string Photo { get; set; }
        [Required]
        public bool IsAvailable { get; set; }
    }
}