using System.ComponentModel.DataAnnotations;

namespace MessageService_API.Dtos
{
    public class MessageDto
    {
        public MessageDto()
        {
            Attachments = new List<MessageAttachmentDto>();
        }

        [Required]
        public Guid Id { get; set; }
        [Required]
        public Guid SenderId { get; set; }
        public string? Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; }
        [Required]
        public bool IsEdited { get; set; }
        public IEnumerable<MessageAttachmentDto> Attachments { get; set; }
    }
}