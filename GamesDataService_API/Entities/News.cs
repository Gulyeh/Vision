using GamesDataService_API.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace GamesDataService_API.Entities
{
    public class News : BaseNews
    {
        public News()
        {
            PhotoId = string.Empty;
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public string PhotoId { get; set; }
        [ForeignKey("GameId")]
        [NotNull]
        public Games? Game { get; set; }
    }
}