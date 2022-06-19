namespace VisionClient.Core.Models
{
    public class NewsModel
    {
        public NewsModel()
        {
            PhotoUrl = string.Empty;
            Title = string.Empty;
            Content = string.Empty;
        }
        public Guid Id { get; set; }
        public string PhotoUrl { get; set; }
        private DateTime createdDate;
        public DateTime CreatedDate 
        {
            get => createdDate;
            set => createdDate = value.ToLocalTime();
        }
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid GameId { get; set; }
    }
}
