using VisionClient.Core.Enums;

namespace VisionClient.Core.Dtos
{
    public class UserChangedStatusDto
    {
        public Guid UserId { get; set; }
        public Status Status { get; set; }
    }
}
