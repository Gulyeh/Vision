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
        public Guid Id { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid GameId { get; set; }
    }
}
