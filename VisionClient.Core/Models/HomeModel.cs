namespace VisionClient.Core.Models
{
    public class HomeModel
    {
        public HomeModel()
        {
            PhotoUrl = string.Empty;
            Title = string.Empty;
            Description = string.Empty;
            Requirements = new RequirementsModel();
            ProductInfo = new ProductInfoModel();
        }

        public string PhotoUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public RequirementsModel Requirements { get; set; }
        public ProductInfoModel ProductInfo { get; set; }
    }
}
