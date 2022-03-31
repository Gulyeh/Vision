using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodesService_API.Dtos
{
    public class CodesDto
    {
        public CodesDto(int codeValue)
        {
            CodeValue = codeValue;
        }
        [Required]
        public int CodeValue { get; set; }
    }
}