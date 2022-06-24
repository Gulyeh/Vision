using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionClient.Core.Dtos
{
    public class EditPaymentDto
    {
        public EditPaymentDto()
        {
            Photo = string.Empty;
        }

        public Guid Id { get; set; }
        public string Photo { get; set; }
        public bool IsAvailable { get; set; }
    }
}
