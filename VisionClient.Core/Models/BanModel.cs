namespace VisionClient.Core.Models
{
    public class BanModel
    {
        public BanModel()
        {
            Reason = string.Empty;
        }

        private DateTime banDate;
        public DateTime BanDate
        {
            get => banDate;
            set => banDate = value.ToLocalTime();
        }

        public string Reason { get; set; }

        private DateTime banExpires;
        public DateTime BanExpires
        {
            get => banExpires;
            set => banExpires = value.ToLocalTime();
        }
    }
}
