using GamesDataService_API.Helpers;
using System.ComponentModel.DataAnnotations;

namespace GamesDataService_API.Dtos
{
    public class AddGamesDto
    {
        public AddGamesDto()
        {
            CoverPhoto = string.Empty;
            IconPhoto = string.Empty;
            BannerPhoto = string.Empty;
            Name = string.Empty;
            Details = string.Empty;
        }

        [Required]
        public string Name { get; set; }
        [Required]
        public string CoverPhoto { get; set; }
        [Required]
        public string IconPhoto { get; set; }
        [Required]
        public string BannerPhoto { get; set; }
        [Required]
        public bool IsAvailable { get; set; }
        [Required]
        public bool IsPurchasable { get; set; }
        [Required]
        public string Details { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Discount { get; set; }
        [Required]
        public InformationsDto? Informations { get; set; }
        [Required]
        public RequirementsDto? Requirements { get; set; }

    }
}