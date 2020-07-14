
using System.Text.RegularExpressions;

namespace SwishMapper.Extensions
{
    public static class StringExtensions
    {
        private static readonly Regex Whitespace = new Regex(@"\s+", RegexOptions.Compiled);


        public static string Crush(this string s)
        {
            return Whitespace.Replace(s, string.Empty);
        }
    }
}
