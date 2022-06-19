using System.Text.RegularExpressions;

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
        internal static bool PasswordMatch(this string data, string RepeatPassword)
        {
            if (data.Equals(RepeatPassword)) return true;
            return false;
        }

        internal static bool IsNumeric(this string data)
        {
            Regex regex = new Regex("[^0-9]+");
            return regex.IsMatch(data);
        }
    }
}
