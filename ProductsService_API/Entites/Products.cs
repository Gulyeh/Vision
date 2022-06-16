using ProductsService_API.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ProductsService_API.Entites
{
    public class Products : BaseProducts
    {
        public Products()
        {
            PhotoUrl = string.Empty;
            PhotoId = string.Empty;
        }

        [Required]
        public string PhotoUrl { get; set; }
        [Required]
        public string PhotoId { get; set; }
        public Guid GameProductId { get; set; }
        [ForeignKey("GameProductId")]
        [NotNull]
        public Games? Game { get; set; }
    }
}