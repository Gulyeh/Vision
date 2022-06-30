namespace VisionClient.Core.Dtos
{
    public class KickUserDto
    {
        public KickUserDto()
        {
            Reason = string.Empty;
        }

        public Guid UserId { get; set; }
        public string Reason { get; set; }
    }
}
