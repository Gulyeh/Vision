using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodesService_API.Dtos
{
    public class GetUserUsedCodesDto
    {
        public GetUserUsedCodesDto()
        {
            Code = string.Empty;
            CodeType = string.Empty;
        }

        public Guid Id { get; set; }
        public string Code { get; set; }
        public string CodeType { get; set; }
    }
}