using GamesDataService_API.Helpers;
using System.ComponentModel.DataAnnotations;

namespace GamesDataService_API.Entities
{
    public class Games : BaseGames
    {
        public Games()
        {
            CoverId = string.Empty;
            IconId = string.Empty;
            BannerId = string.Empty;
            News = new List<News>();
            Informations = new();
            Requirements = new();
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        public string IconId { get; set; }
        [Required]
        public string CoverId { get; set; }
        [Required]
        public string BannerId { get; set; }
        public ICollection<News> News { get; set; }
        [Required]
        public Informations Informations { get; set; }
        [Required]
        public Requirements Requirements { get; set; }
    }
}