using System.ComponentModel.DataAnnotations;

namespace GamesDataService_API.Helpers
{
    public class BaseGames
    {
        public BaseGames()
        {
            Name = string.Empty;
            IconUrl = string.Empty;
            CoverUrl = string.Empty;
            BannerUrl = string.Empty;
        }

        [Required]
        public string Name { get; set; }
        [Required]
        public string IconUrl { get; set; }
        [Required]
        public string CoverUrl { get; set; }
        [Required]
        public string BannerUrl { get; set; }
        [Required]
        public bool IsAvailable { get; set; }
    }
}