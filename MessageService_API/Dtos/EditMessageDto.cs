using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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