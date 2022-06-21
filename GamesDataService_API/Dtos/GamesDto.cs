using GamesDataService_API.Helpers;
using System.ComponentModel.DataAnnotations;

namespace GamesDataService_API.Dtos
{
    public class GamesDto : BaseGames
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public InformationsDto? Informations { get; set; }
        [Required]
        public RequirementsDto? Requirements { get; set; }
    }
}