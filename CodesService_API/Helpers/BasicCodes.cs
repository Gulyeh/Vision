using System.ComponentModel.DataAnnotations;
using CodesService_API.Entites;

namespace CodesService_API.Helpers
{
    public class BasicCodes
    {
        public BasicCodes()
        {
            Code = string.Empty;
            CodeValue = string.Empty;
        }

        [Required]
        public string Code { get; set; }
        [Required]
        public string CodeValue { get; set; }
        [Required]
        public DateTime ExpireDate { get; set; }
        [Required]
        public CodeTypes CodeType { get; set; }
        [Required]
        public bool isLimited { get; set; }
        public int? Uses { get; set; }
        public Guid? gameId { get; set; }
    }
}