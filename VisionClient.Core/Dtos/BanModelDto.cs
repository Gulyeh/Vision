namespace VisionClient.Core.Dtos
{
    public class BanModelDto
    {
        public BanModelDto()
        {
            Reason = string.Empty;
        }

        public Guid UserId { get; set; }
        public string Reason { get; set; }
        public DateTime BanExpires { get; set; } = DateTime.Now;

        public bool Validation() => UserId != Guid.Empty && !string.IsNullOrWhiteSpace(Reason) && BanExpires > DateTime.Now;
    }
}
