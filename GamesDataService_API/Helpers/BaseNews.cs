using System.ComponentModel.DataAnnotations;

namespace GamesDataService_API.Helpers
{
    public class BaseNews
    {
        public BaseNews()
        {
            Title = string.Empty;
            PhotoUrl = string.Empty;
            Content = string.Empty;
        }

        [Required]
        public string Title { get; set; }
        [Required]
        public string PhotoUrl { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public Guid GameId { get; set; }

    }
}