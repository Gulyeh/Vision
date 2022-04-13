using GamesDataService_API.Helpers;
using System.ComponentModel.DataAnnotations;

namespace GamesDataService_API.Dtos
{
    public class GamesDto : BaseGames
    {
        [Required]
        public Guid Id { get; set; }
        public IFormFile? CoverPhoto { get; set; }
        public IFormFile? IconPhoto { get; set; }
    }
}