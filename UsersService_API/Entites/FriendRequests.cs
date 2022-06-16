using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
        [JsonIgnore]
        public DateTime RequestDate { get; set; } = DateTime.Now;
    }
}