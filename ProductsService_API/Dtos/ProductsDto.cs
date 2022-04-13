using ProductsService_API.Helpers;
using System.ComponentModel.DataAnnotations;

namespace ProductsService_API.Dtos
{
    public class ProductsDto : BaseProducts
    {
        public ProductsDto()
        {
            PhotoUrl = string.Empty;
        }

        [Required]
        public Guid ProductId { get; set; }
        [Required]
        public Guid GameId { get; set; }
        [Required]
        public string PhotoUrl { get; set; }
        public IFormFile? Photo { get; set; }
    }
}