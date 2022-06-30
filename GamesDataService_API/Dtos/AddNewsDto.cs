using System.ComponentModel.DataAnnotations;

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