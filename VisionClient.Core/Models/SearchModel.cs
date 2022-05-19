using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionClient.Core.Models
{
    public class SearchModel
    {
        public SearchModel()
        {
            User = new();
        }

        public UserModel User { get; set; }
        public bool IsFriend { get; set; }
    }
}
