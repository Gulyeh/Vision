using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionClient.Core.Models.Account
{
    public class BanModel
    {
        private DateTime banDate;
        public DateTime BanDate 
        { 
            get => banDate;
            set => banDate = value.ToLocalTime();
        }
        public string? Reason { get; set; }
        private DateTime banExpires;
        public DateTime BanExpires 
        {
            get => banExpires;
            set => banExpires = value.ToLocalTime();
        }
    }
}
