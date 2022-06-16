using GamesDataService_API.Helpers;
using System.ComponentModel.DataAnnotations;

namespace GamesDataService_API.Dtos
{
    public class NewsDto : BaseNews
    {
        [Required]
        public Guid Id { get; set; }
        public IFormFile? Photo { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
    }
}