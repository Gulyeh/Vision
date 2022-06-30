using System.ComponentModel.DataAnnotations;

namespace GameAccessService_API.Dtos
{
    public class AccessDataDto
    {
        [Required]
        public Guid GameId { get; set; }
        [Required]
        public Guid UserId { get; set; }
    }
}