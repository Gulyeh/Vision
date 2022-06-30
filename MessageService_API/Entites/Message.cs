using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageService_API.Entites
{
    public class Message
    {
        public Message()
        {
            Chat = new Chat();
            Attachments = new List<MessageAttachment>();
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid SenderId { get; set; }
        [Required]
        public Guid ReceiverId { get; set; }
        public string? Content { get; set; }
        public DateTime? DateRead { get; set; }
        [Required]
        public DateTime MessageSent { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
        [Required]
        public bool SenderDeleted { get; set; }
        [Required]
        public bool ReceiverDeleted { get; set; }
        [Required]
        public bool IsEdited { get; set; }
        [Required]
        public Guid ChatId { get; set; }
        [ForeignKey("ChatId")]
        public Chat Chat { get; set; }
        public ICollection<MessageAttachment> Attachments { get; set; }
    }
}