namespace VisionClient.Core.Models
{
    public class NewsModel
    {
        public NewsModel()
        {
            PhotoUrl = String.Empty;
            Title = String.Empty;
            Content = String.Empty;
        }

        public string PhotoUrl { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
