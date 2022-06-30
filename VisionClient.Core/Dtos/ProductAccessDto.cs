namespace VisionClient.Core.Dtos
{
    public class ProductAccessDto
    {
        public bool IsSuccess { get; set; }
        public Guid GameId { get; set; }
        public Guid ProductId { get; set; }
    }
}
