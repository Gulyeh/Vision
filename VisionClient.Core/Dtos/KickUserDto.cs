using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionClient.Core.Dtos
{
    public class KickUserDto
    {
        public KickUserDto()
        {
            Reason = string.Empty;
        }

        public Guid UserId { get; set; }
        public string Reason { get; set; }
    }
}
