using System;

namespace FrameworkUtils.Utils
{
    public static class StringHelper
    {
        public static bool In(this string str, bool ignoreCase, params string[] args)
        {
            if (str != null)
            {
                foreach (string arg in args)
                {
                    if (ignoreCase == true)
                    {
                        if (str.Equals(arg, StringComparison.OrdinalIgnoreCase))
                            return true;
                    }
                    else
                    {
                        if (arg.Equals(str))
                            return true;
                    }
                }
            }
            return false;
        }

        public static string Truncate(string input, int maxLength, bool showEllipsis = false)
        {
            if (maxLength < 0)
                maxLength = 0;

            if (string.IsNullOrEmpty(input) || input.Length <= maxLength)
                return input;

            if (showEllipsis && maxLength > 2)
                return input.Substring(0, maxLength - 3) + "...";
            else
                return input.Substring(0, maxLength);
        }
    }
}
