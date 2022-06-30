namespace GamesDataService_API.Messages
{
    public class NewProductDto
    {
        public NewProductDto()
        {
            PhotoUrl = string.Empty;
            PhotoId = string.Empty;
            Title = string.Empty;
            Details = string.Empty;
        }

        public Guid GameId { get; set; }
        public string PhotoUrl { get; set; }
        public string PhotoId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public int Discount { get; set; }
        public string Details { get; set; }
    }
}