using GamesDataService_API.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace GamesDataService_API.Dtos
{
    public class AddNewsDto : BaseNews
    {
        [Required]
        [NotNull]
        public IFormFile? Photo { get; set; }
    }
}