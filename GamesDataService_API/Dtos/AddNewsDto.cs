using GamesDataService_API.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace GamesDataService_API.Dtos
{
    public class AddNewsDto
    {
        public AddNewsDto()
        {
            Photo = string.Empty;
            Title = string.Empty;
            Content = string.Empty;
        }

        [Required]
        public string Photo { get; set; }
        [Required]
        public Guid GameId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
    }
}