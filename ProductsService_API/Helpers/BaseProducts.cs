using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductsService_API.Helpers
{
    public class BaseProducts
    {
        public BaseProducts()
        {
            Title = string.Empty;
            Discount = 0;
        }

        [Required]
        public string Title { get; set; }
        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal Price { get; set; }
        [Required]
        public bool IsAvailable { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? Discount { get; set; }
    }
}