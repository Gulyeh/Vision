namespace VisionClient.Core.Models
{
    public class ProductInfoModel
    {
        public ProductInfoModel()
        {
            Genre = string.Empty;
            Developer = string.Empty;
            Publisher = string.Empty;
            Language = string.Empty;
            Description = string.Empty;
        }

        public string Genre { get; set; }
        public string Developer { get; set; }
        public string Publisher { get; set; }
        public string Language { get; set; }
        public string Description { get; set; }
    }
}
