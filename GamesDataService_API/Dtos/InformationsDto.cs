using System.ComponentModel.DataAnnotations;

namespace GamesDataService_API.Dtos
{
    public class InformationsDto
    {
        public InformationsDto()
        {
            Genre = string.Empty;
            Developer = string.Empty;
            Publisher = string.Empty;
            Language = string.Empty;
            Description = string.Empty;
        }

        [Required]
        public string Description { get; set; }
        [Required]
        public string Genre { get; set; }
        [Required]
        public string Developer { get; set; }
        [Required]
        public string Publisher { get; set; }
        [Required]
        public string Language { get; set; }
    }
}