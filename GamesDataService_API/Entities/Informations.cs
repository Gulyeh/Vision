using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace GamesDataService_API.Entities
{
    public class Informations
    {
        public Informations()
        {
            Genre = string.Empty;
            Developer = string.Empty;
            Publisher = string.Empty;
            Language = string.Empty;
            Description = string.Empty;
        }

        [Key]
        public Guid Id { get; set; }
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
        [Required]
        public Guid GameId { get; set; }
        [ForeignKey("GameId")]
        [NotNull]
        public Games? Game { get; set; }
    }
}