using GameAccessService_API.Helpers;
using System.ComponentModel.DataAnnotations;

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