using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodesService_API.Dtos
{
    public class CodesDataDto
    {
        public CodesDataDto()
        {
            Id = string.Empty;
            Code = string.Empty;
            CodeValue = string.Empty;
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