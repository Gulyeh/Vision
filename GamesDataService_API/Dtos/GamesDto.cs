using GamesDataService_API.Helpers;
using System.ComponentModel.DataAnnotations;

namespace GamesDataService_API.Dtos
{
    public class GamesDto : BaseGames
    {
        public GamesDto()
        {
            CoverPhoto = string.Empty;
            IconPhoto = string.Empty;
            BannerPhoto = string.Empty;
        }

        [Required]
        public Guid Id { get; set; }
        public string CoverPhoto { get; set; }
        public string IconPhoto { get; set; }
        public string BannerPhoto { get; set; }
        [Required]
        public InformationsDto? Informations { get; set; }
        [Required]
        public RequirementsDto? Requirements { get; set; }
    }
}