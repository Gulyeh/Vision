using ProductsService_API.Helpers;
using System.ComponentModel.DataAnnotations;

namespace ProductsService_API.Dtos
{
    public class AddGamesDto : BaseProducts
    {
        [Required]
        public Guid GameId { get; set; }
    }
}