using System.ComponentModel.DataAnnotations;
using GameAccessService_API.Helpers;

namespace GameAccessService_API.Entites
{
    public class UserProducts : BaseUser
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid ProductId { get; set; }
    }
}