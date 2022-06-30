using System.ComponentModel.DataAnnotations;

namespace ProductsService_API.Dtos
{
    public class EditCurrencyDto
    {
        public EditCurrencyDto()
        {
            Title = string.Empty;
            Details = string.Empty;
        }

        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public int Amount { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public bool IsAvailable { get; set; }
        public int Discount { get; set; }
        [Required]
        public string Details { get; set; }
    }
}