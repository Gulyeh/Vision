namespace VisionClient.Core.Dtos
{
    public class BanGameDto
    {
        public BanGameDto()
        {
            Reason = string.Empty;
        }

        public Guid UserId { get; set; }
        public Guid GameId { get; set; }
        public string Reason { get; set; }
        public DateTime BanExpires { get; set; } = DateTime.Now;

        public bool Validation() => UserId != Guid.Empty && !string.IsNullOrWhiteSpace(Reason) && BanExpires > DateTime.Now && GameId != Guid.Empty;
    }
}
