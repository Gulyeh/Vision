using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using ProductsService_API.Helpers;

namespace ProductsService_API.Dtos
{
    public class AddProductsDto : BaseProducts
    {
        [Required]
        public Guid GameId { get; set; }
        [Required]
        [NotNull]
        public IFormFile? Photo { get; set; }
    }
}