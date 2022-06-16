using System.ComponentModel.DataAnnotations;

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
        public string CodeValue { get; set; }
        [Required]
        public DateTime ExpireDate { get; set; }
        [Required]
        public CodeTypes CodeType { get; set; }
        public Signatures? Signature { get; set; }
        [Required]
        public bool IsLimited { get; set; }
        public int? Uses { get; set; }
        public Guid? GameId { get; set; }
    }
}