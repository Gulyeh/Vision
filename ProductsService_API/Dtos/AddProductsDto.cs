using ProductsService_API.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

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