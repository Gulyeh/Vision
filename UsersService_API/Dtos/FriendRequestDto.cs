using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UsersService_API.Dtos
{
    public class FriendRequestDto
    {
        [Required]
        public Guid Receiver { get; set; }
        [JsonIgnore]
        public Guid Sender { get; set; }
        [JsonIgnore]
        public DateTime RequestDate { get; init; } = DateTime.Now;
    }
}