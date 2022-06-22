using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Helpers;

namespace VisionClient.Core.Dtos
{
    public class EditCurrencyDto : AddCurrencyDto
    {     
        public Guid Id { get; set; }
    }
}
