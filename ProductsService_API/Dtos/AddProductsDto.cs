using System.ComponentModel.DataAnnotations;

namespace ProductsService_API.Dtos
{
    public class AddProductsDto
    {
        public AddProductsDto()
        {
            Photo = string.Empty;
            Title = string.Empty;
            Details = string.Empty;
        }

        [Required]
        public Guid GameId { get; set; }
        [Required]
        public string Photo { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public bool IsAvailable { get; set; }
        public int Discount { get; set; }
        [Required]
        public string Details { get; set; }
    }
}