using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsService_API.Dtos
{
    public class AddCurrencyDto
    {
        public AddCurrencyDto()
        {
            Title = string.Empty;
            Details = string.Empty;         
        }

        [Required]
        public string Title { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public bool IsAvailable { get; set; }
        public int Discount { get; set; }
        [Required]
        public string Details { get; set; }
    }
}