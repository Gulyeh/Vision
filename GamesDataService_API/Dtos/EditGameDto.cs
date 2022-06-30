using System.ComponentModel.DataAnnotations;

namespace GamesDataService_API.Dtos
{
    public class EditGameDto
    {
        public EditGameDto()
        {
            CoverPhoto = string.Empty;
            IconPhoto = string.Empty;
            BannerPhoto = string.Empty;
            Name = string.Empty;
            Informations = new();
            Requirements = new();
        }

        [Required]
        public Guid Id { get; set; }
        public string CoverPhoto { get; set; }
        public string IconPhoto { get; set; }
        public string BannerPhoto { get; set; }
        [Required]
        public InformationsDto Informations { get; set; }
        [Required]
        public RequirementsDto Requirements { get; set; }
        [Required]
        public bool IsAvailable { get; set; }
        [Required]
        public string Name { get; set; }
    }
}