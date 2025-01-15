namespace FrameworkUtils.Utils
{
    public static class PathHelper
    {
        #region SanitizeFileName

        /// <summary>
        /// Clean up a file name from invalid characters.
        /// 
        /// https://stackoverflow.com/questions/309485/c-sharp-sanitize-file-name
        /// </summary>
        public static string SanitizeFileName(string fileName, string replaceInvalidCharsWith = "_")
        {
            if (string.IsNullOrEmpty(fileName))
                return "";

            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(GetWinInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(fileName, invalidRegStr, replaceInvalidCharsWith);
        }

        #endregion

        #region SanitizeFilePath

        /// <summary>
        /// Replaces illegal characters from the path with the specified character.
        /// </summary>
        public static string SanitizeFilePath(string filepath, string replaceInvalidCharsWith = "_")
        {
            if (string.IsNullOrEmpty(filepath))
                return filepath;

            char[] invalidFileNameChars = GetWinInvalidFileNameChars();
            string[] filepathPortions = filepath.Split('\\', '/');
            string[] sanitizedPortions = new string[filepathPortions.Length];
            for (int i = 0; i < filepathPortions.Length; i++)
            {
                sanitizedPortions[i] = string.Join(replaceInvalidCharsWith, filepathPortions[i].Split(invalidFileNameChars));
            }
            return string.Join("\\", sanitizedPortions);
        }

        #endregion

        #region GetWinInvalidFileNameChars

        /// <summary>
        /// Returns file name invalid characters that are forbidden on Windows platform.
        /// 
        /// NOTE: The built-in method System.IO.Path.GetInvalidFileNameChars() cannot be used on Android as it returns only two characters!
        /// </summary>
        public static char[] GetWinInvalidFileNameChars()
        {
            return new char[] {
                '\u0000',
                '\u0001',
                '\u0002',
                '\u0003',
                '\u0004',
                '\u0005',
                '\u0006',
                '\u0007',
                '\u0008',
                '\u0009',  // '\t'
                '\u000A',  // '\n'
                '\u000B',
                '\u000C',
                '\u000D',  // '\r'
                '\u000E',
                '\u000F',
                '\u0010',
                '\u0011',
                '\u0012',
                '\u0013',
                '\u0014',
                '\u0015',
                '\u0016',
                '\u0017',
                '\u0018',
                '\u0019',
                '\u001A',
                '\u001B',
                '\u001C',
                '\u001D',
                '\u001E',
                '\u001F',
                '<',
                '>',
                ':',
                '"',
                '/',
                '\\',
                '|',
                '?',
                '*'};
        }

        #endregion
    }
}
