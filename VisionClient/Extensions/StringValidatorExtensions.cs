using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VisionClient.Extensions
{
    internal static class StringValidatorExtensions
    {
        internal static bool ValidateEmailAddress(this string data)
        {
            Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            return regex.IsMatch(data);
        }
        internal static bool ValidatePassword(this string data)
        {
            Regex regex = new Regex(@"^.{8,15}$");
            return regex.IsMatch(data);
        }
    }
}
