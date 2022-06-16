using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MessageService_API.Dtos
{
    public class EditMessageDto
    {
        public EditMessageDto()
        {
            DeletedAttachmentsId = new List<Guid>();
        }

        [Required]
        public Guid ChatId { get; set; }
        [Required]
        public Guid MessageId { get; set; }
        [JsonIgnore]
        public Guid SenderId { get; set; }
        public string? Content { get; set; }
        public IEnumerable<Guid> DeletedAttachmentsId { get; set; }
    }
}