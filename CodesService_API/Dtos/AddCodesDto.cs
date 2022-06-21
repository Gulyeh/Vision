using System.ComponentModel.DataAnnotations;
using CodesService_API.Helpers;

namespace CodesService_API.Dtos
{
    public class AddCodesDto
    {
        public AddCodesDto()
        {
            Code = string.Empty;
            CodeValue = string.Empty;
            codeType = string.Empty;
            Signature = string.Empty;
        }

        [Required]
        public string Code { get; set; }
        [Required]
        public string CodeValue { get; set; }
        [Required]
        public DateTime ExpireDate { get; set; }
        private string codeType;
        [Required]
        public string CodeType 
        { 
            get => codeType; 
            set {
                if(value.Equals("Game") || value.Equals("Package")) codeType = "Product";
                else codeType = value;
            }
        }
        public string Signature { get; set; }
        [Required]
        public bool IsLimited { get; set; }
        [Required]
        public int Uses { get; set; }
        public Guid GameId { get; set; }
    }
}