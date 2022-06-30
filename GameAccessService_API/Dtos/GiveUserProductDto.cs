using System.ComponentModel.DataAnnotations;

namespace GameAccessService_API.Dtos
{
    public class GiveUserProductDto
    {
        [Required]
        public Guid GameId { get; set; }
        public Guid ProductId { get; set; }
        [Required]
        public Guid UserId { get; set; }
    }
}