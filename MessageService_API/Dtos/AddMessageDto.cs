using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MessageService_API.Dtos
{
    public class AddMessageDto
    {
        public AddMessageDto()
        {
            AttachmentsList = new List<string>();
        }

        [JsonIgnore]
        public Guid SenderId { get; set; }
        [JsonIgnore]
        public DateTime? DateRead { get; set; }
        [JsonIgnore]
        public Guid ReceiverId { get; set; }
        public string? Content { get; set; }
        [Required]
        public Guid ChatId { get; set; }
        public IEnumerable<string> AttachmentsList { get; set; }
    }
}