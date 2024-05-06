using System.Text.RegularExpressions;

namespace Sync.Bi.Leads.Helpers
{
    public static class Helper
    {
        private static string patternOnlyNumber = @"\D"; // @"[^0-9]"

        public static string OnlyNumber(this string str)
        {
            return Regex.Replace(str, patternOnlyNumber, "");
        }
    }
}
