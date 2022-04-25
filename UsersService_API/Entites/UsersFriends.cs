using System.ComponentModel.DataAnnotations;

namespace UsersService_API.Entites
{
    public class UsersFriends
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid User1 { get; set; }
        [Required]
        public Guid User2 { get; set; }
        [Required]
        public DateTime FriendsSince { get; init; } = DateTime.UtcNow;
    }
}