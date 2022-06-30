using VisionClient.Core.Enums;

namespace VisionClient.Core.Dtos
{
    public class UserDataDto
    {
        public UserDataDto()
        {
            PhotoUrl = string.Empty;
            Username = string.Empty;
            Description = string.Empty;
        }

        public string Description { get; set; }
        public int CurrencyValue { get; set; }
        public Guid UserId { get; set; }
        public string PhotoUrl { get; set; }
        public Status Status { get; set; }
        public string Username { get; set; }
    }
}
