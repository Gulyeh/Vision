using System.ComponentModel.DataAnnotations;

namespace CodesService_API.Dtos
{
    public class EditCodeDto
    {
        public EditCodeDto()
        {
            Code = string.Empty;
        }

        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public DateTime ExpireDate { get; set; }
        [Required]
        public bool IsLimited { get; set; }
        [Required]
        public int Uses { get; set; }
    }
}