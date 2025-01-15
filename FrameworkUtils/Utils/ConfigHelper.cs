using System.Text.RegularExpressions;
using DevExpress.ExpressApp.Utils;

namespace FrameworkUtils.Utils
{
    public static class ConfigHelper
    {
        /// <summary>
        /// Gets the value of connection string Application Name, if specified.
        /// </summary>
        public static string GetApplicationName(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return "";

            // search for portion: "Application Name=XYZ"
            string pattern = @"(^|;) *Application Name *=(?<appName>[^;]*)";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            // Get Application Name value
            Match matchedAppName = regex.Match(connectionString);
            Group matchedGroup = matchedAppName.Groups["appName"];
            return matchedGroup.Value.Trim();
        }

        /// <summary>
        /// Returns true if the connection string Application Name contains the portion 'test' (case insensitive).
        /// </summary>
        public static bool IsTestConnectionString(string connectionString)
        {
            string appName = GetApplicationName(connectionString);
            return !string.IsNullOrEmpty(appName) && appName.IndexOf("test", System.StringComparison.OrdinalIgnoreCase) > -1;
        }

        public static bool GetBoolValue(string keyName, bool defaultValue = false)
        {
            Guard.ArgumentNotNullOrEmpty(keyName, nameof(keyName));

            bool result = defaultValue;

            string keyValue = System.Configuration.ConfigurationManager.AppSettings[keyName];
            if (bool.TryParse(keyValue, out bool parsed))
                result = parsed;

            return result;
        }
    }
}
