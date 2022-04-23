
using GameAccessService_API.Helpers;
using System.ComponentModel.DataAnnotations;

namespace GameAccessService_API.Entites
{
    public class UserGames : BaseUser
    {
        [Key]
        public int Id { get; set; }
    }
}