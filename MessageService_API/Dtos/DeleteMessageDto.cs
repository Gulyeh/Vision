using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MessageService_API.Dtos
{
    public class DeleteMessageDto
    {
        public Guid ChatId { get; set; }
        public Guid MessageId { get; set; }
        [JsonIgnore]
        public Guid userId { get; set; }
    }
}