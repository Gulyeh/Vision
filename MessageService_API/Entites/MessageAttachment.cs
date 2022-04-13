using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageService_API.Entites
{
    public class MessageAttachment
    {
        public MessageAttachment()
        {
            AttachmentUrl = string.Empty;
            AttachmentId = string.Empty;
            Message = new Message();
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        public string AttachmentUrl { get; set; }
        [Required]
        public string AttachmentId { get; set; }
        [Required]
        public Guid MessageId { get; set; }
        [ForeignKey("MessageId")]
        public Message Message { get; set; }
    }
}