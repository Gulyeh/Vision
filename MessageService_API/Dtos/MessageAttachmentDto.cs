using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageService_API.Dtos
{
    public class MessageAttachmentDto
    {
        public MessageAttachmentDto()
        {
            AttachmentUrl = string.Empty;
        }

        public Guid Id { get; set; }
        public string AttachmentUrl { get; set; }
    }
}