using System.ComponentModel.DataAnnotations;

namespace GamesDataService_API.Dtos
{
    public class EditNewsDto
    {
        public EditNewsDto()
        {
            Photo = string.Empty;
            Content = string.Empty;
            Title = string.Empty;
        }

        [Required]
        public Guid Id { get; set; }
        public string Photo { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public Guid GameId { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public string Title { get; set; }
    }
}