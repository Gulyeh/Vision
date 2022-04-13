using System.ComponentModel.DataAnnotations;

namespace MessageService_API.Dtos
{
    public class EditMessageDto
    {
        [Required]
        public Guid ChatId { get; set; }
        [Required]
        public Guid MessageId { get; set; }
        public string? Content { get; set; }
        public IEnumerable<Guid>? DeletedAttachmentsId { get; set; }
    }
}