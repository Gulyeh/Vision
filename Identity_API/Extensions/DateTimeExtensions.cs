using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity_API.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToShortDate(this DateTime time){
            var date = time.ToShortDateString();
            var hour = time.ToShortTimeString();
            StringBuilder builder = new StringBuilder();
            builder.Append(date);
            builder.Append(' ');
            builder.Append(hour);
            return builder.ToString();
        }
    }
}