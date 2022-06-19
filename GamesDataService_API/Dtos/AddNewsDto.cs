using GamesDataService_API.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace GamesDataService_API.Dtos
{
    public class AddNewsDto : BaseNews
    {
        public AddNewsDto()
        {
            Photo = string.Empty;
        }

        [Required]
        public string Photo { get; set; }
        [JsonIgnore]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [Required]
        public Guid GameId { get; set; }
    }
}