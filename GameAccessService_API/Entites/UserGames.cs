
using GameAccessService_API.Helpers;
using System.ComponentModel.DataAnnotations;

namespace GameAccessService_API.Entites
{
    public class UserGames : BaseUser
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public Guid GameId { get; set; }
    }
}