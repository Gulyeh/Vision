using GamesDataService_API.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace GamesDataService_API.Dtos
{
    public class AddGamesDto : BaseGames
    {
        [Required]
        [NotNull]
        public IFormFile? CoverPhoto { get; set; }
        [Required]
        [NotNull]
        public IFormFile? IconPhoto { get; set; }
    }
}