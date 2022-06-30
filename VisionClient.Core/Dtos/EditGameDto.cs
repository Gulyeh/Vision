using VisionClient.Core.Models;

namespace VisionClient.Core.Dtos
{
    public class EditGameDto
    {
        public EditGameDto()
        {
            CoverPhoto = string.Empty;
            IconPhoto = string.Empty;
            BannerPhoto = string.Empty;
            Name = string.Empty;
            Informations = new();
            Requirements = new();
        }

        public Guid Id { get; set; }
        public string CoverPhoto { get; set; }
        public string IconPhoto { get; set; }
        public string BannerPhoto { get; set; }
        public bool IsAvailable { get; set; }
        public string Name { get; set; }
        public ProductInfoModel Informations { get; set; }
        public RequirementsModel Requirements { get; set; }
    }
}
