using System.Text.Json.Serialization;

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