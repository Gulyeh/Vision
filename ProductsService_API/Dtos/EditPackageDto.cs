using System.ComponentModel.DataAnnotations;

namespace ProductsService_API.Dtos
{
    public class EditPackageDto
    {
        public EditPackageDto()
        {
            Title = string.Empty;
            Discount = 0;
            Details = string.Empty;
            Photo = string.Empty;
        }

        [Required]
        public Guid Id { get; set; }
        [Required]
        public Guid GameId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public bool IsAvailable { get; set; }
        public int Discount { get; set; }
        [Required]
        public string Details { get; set; }
        public string Photo { get; set; }
    }
}