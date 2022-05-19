using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionClient.Core.Models
{
    public class AttachmentModel : ICloneable
    {
        public AttachmentModel()
        {
            PhotoUrl = string.Empty;
        }

        public int Id { get; set; }
        public string PhotoUrl { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
