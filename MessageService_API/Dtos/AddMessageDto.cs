using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MessageService_API.Dtos
{
    public class AddMessageDto
    {
        [JsonIgnore]
        public Guid SenderId { get; set; }
        [JsonIgnore]
        public DateTime? DateRead { get; set; }
        [Required]
        public Guid ReceiverId { get; set; }
        public string? Content { get; set; }
        public Guid ChatId { get; set; }
        public ICollection<IFormFile>? Attachments { get; set; }
    }
}