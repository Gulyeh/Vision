using GamesDataService_API.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace GamesDataService_API.Entities
{
    public class Games : BaseGames
    {
        public Games()
        {
            CoverId = string.Empty;
            IconId = string.Empty;
            BannerId = string.Empty;
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        public string IconId { get; set; }
        [Required]
        public string CoverId { get; set; }
        [Required]
        public string BannerId { get; set; }
        [Required]
        public bool IsAvailable { get; set; }
        public ICollection<News>? News { get; set; }
        [Required]
        [NotNull]
        public Informations? Informations { get; set; }
        [Required]
        [NotNull]
        public Requirements? Requirements { get; set; }
    }
}