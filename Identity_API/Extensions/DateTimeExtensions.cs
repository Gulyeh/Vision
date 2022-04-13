using System.Text;

namespace Identity_API.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToShortDate(this DateTime time)
        {
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