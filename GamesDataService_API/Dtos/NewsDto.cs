using GamesDataService_API.Helpers;
using System.ComponentModel.DataAnnotations;

namespace GamesDataService_API.Dtos
{
    public class NewsDto : BaseNews
    {
        public NewsDto()
        {
            Photo = string.Empty;
        }

        [Required]
        public Guid Id { get; set; }
        public string Photo { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public Guid GameId { get; set; }
    }
}