using ProductsService_API.Helpers;
using System.ComponentModel.DataAnnotations;

namespace ProductsService_API.Dtos
{
    public class GamesDto : BaseProducts
    {
        [Required]
        public Guid GameId { get; set; }
        public ICollection<ProductsDto>? GameProducts { get; set; }
    }
}