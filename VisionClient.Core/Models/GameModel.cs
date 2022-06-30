namespace VisionClient.Core.Models
{
    public class GameModel : ICloneable
    {
        public GameModel()
        {
            Name = string.Empty;
            IconUrl = string.Empty;
            CoverUrl = string.Empty;
            BannerUrl = string.Empty;
            Requirements = new();
            Informations = new();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public string CoverUrl { get; set; }
        public string BannerUrl { get; set; }
        public bool IsAvailable { get; set; }
        public RequirementsModel Requirements { get; set; }
        public ProductInfoModel Informations { get; set; }

        public object Clone() => MemberwiseClone();
    }
}
