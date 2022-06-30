using ProductsService_API.Helpers;
using System.ComponentModel.DataAnnotations;

namespace ProductsService_API.Dtos
{
    public class GamesDto : BaseProducts
    {
        public GamesDto()
        {
            PhotoUrl = string.Empty;
        }

        [Required]
        public Guid GameId { get; set; }
        [Required]
        public string PhotoUrl { get; set; }
        [Required]
        public bool IsPurchased { get; set; }
        public ICollection<ProductsDto>? Products { get; set; }
    }
}