using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
        public DateTime FriendsSince { get; set; }
    }
}