using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UsersService_API.Entites
{
    public class FriendRequests
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid Sender { get; set; }
        [Required]
        public Guid Receiver { get; set; }
        [Required]
        public DateTime RequestDate { get; set; }
    }
}