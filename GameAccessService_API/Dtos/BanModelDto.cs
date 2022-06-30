namespace GameAccessService_API.Dtos
{
    public class BanModelDto
    {
        public BanModelDto()
        {
            Reason = string.Empty;
        }

        public string Reason { get; set; }
        public DateTime BanDate { get; set; }
        public DateTime BanExpires { get; set; }
    }
}