using System.ComponentModel.DataAnnotations;

namespace UsersService_API.Entites
{
    public class BlockedUsers
    {
        public BlockedUsers(Guid blockerId, Guid blockedId)
        {
            BlockerId = blockerId;
            BlockedId = blockedId;
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid BlockerId { get; set; }
        [Required]
        public Guid BlockedId { get; set; }
    }
}