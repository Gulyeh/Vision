namespace VisionClient.Core.Dtos
{
    public class EditNewsDto
    {
        public EditNewsDto()
        {
            Photo = string.Empty;
            Content = string.Empty;
            Title = string.Empty;
        }

        public Guid Id { get; set; }
        public string Photo { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid GameId { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
    }
}
