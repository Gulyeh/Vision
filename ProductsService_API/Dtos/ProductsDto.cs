using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ProductsService_API.Helpers;

namespace ProductsService_API.Dtos
{
    public class ProductsDto : BaseProducts
    {
        [Required]
        public Guid Id { get; set; }
        public string? PhotoUrl { get; set; }
        public IFormFile? Photo { get; set; }
    }
}