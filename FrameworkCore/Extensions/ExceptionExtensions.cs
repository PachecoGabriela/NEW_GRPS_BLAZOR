using System;
using System.Text;

namespace FrameworkCore.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetFullExceptionText(this Exception exception, bool includeStackTrace = true)
        {
            StringBuilder builder = new StringBuilder();

            string indent = "";
            Exception e = exception;
            while (e != null)
            {
                if (e != exception)
                {
                    builder.AppendLine();
                    builder.AppendLine(indent + "Inner Exception:");
                }

                builder.AppendLine(indent + e.Message);
                if (includeStackTrace)
                    builder.AppendLine(indent + e.StackTrace);

                e = e.InnerException;
                indent += "\t";
            }

            return builder.ToString();
        }
    }
}
