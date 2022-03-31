using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodesService_API.Dtos
{
    public class CodesDataDto
    {
        public CodesDataDto(string id, string code, string codeValue, DateTime expireDate)
        {
            Id = id;
            Code = code;
            CodeValue = codeValue;
            ExpireDate = expireDate;
        }

        [Required]
        public string Id { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string CodeValue { get; set; }
        [Required]
        public DateTime ExpireDate { get; set; }
    }
}