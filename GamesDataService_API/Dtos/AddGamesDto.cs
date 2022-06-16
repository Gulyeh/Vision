using GamesDataService_API.Helpers;
using System.ComponentModel.DataAnnotations;

namespace GamesDataService_API.Dtos
{
    public class AddGamesDto : BaseGames
    {
        [Required]
        public IFormFile? CoverPhoto { get; set; }
        [Required]
        public IFormFile? IconPhoto { get; set; }
        [Required]
        public IFormFile? BannerPhoto { get; set; }
        [Required]
        public InformationsDto? Informations { get; set; }
        [Required]
        public RequirementsDto? Requirements { get; set; }
    }
}