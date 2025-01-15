using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkUtils.Controllers
{
    public class LoggingController : IDisposable
    {
        public event GetFileNameHandler GetCustomFileName;

        public LoggingLevel MinimumLoggingLevel { get; set; }
        public LoggingTimeFormat TimeFormat { get; set; }
        public string LogDirectory { get; private set; }
        public int MaxEntries { get; set; }

        private StreamWriter writer = null;
        private int entrycount = 0;

        public LoggingController(string folder) : this(folder, LoggingLevel.Info, LoggingTimeFormat.TwentyFourHour) { }
        public LoggingController(string folder, LoggingLevel loglevel) : this(folder, loglevel, LoggingTimeFormat.TwentyFourHour) { }
        public LoggingController(string folder, LoggingLevel loglevel, LoggingTimeFormat format)
        {
            TimeFormat = format;
            LogDirectory = folder;
            MinimumLoggingLevel = loglevel;
            MaxEntries = 10000;
        }

        public void CreateLogFile()
        {
            try
            {
                if (writer != null)
                {
                    writer.Flush();
                    writer.Dispose();
                    writer = null;
                }

                if (!Directory.Exists(LogDirectory))
                    Directory.CreateDirectory(LogDirectory);

                string filename = DateTime.Now.ToString("MMMddyyyy-HHmmss") + ".log";

                if (GetCustomFileName != null)
                {
                    string name = GetCustomFileName(this);
                    if (!string.IsNullOrEmpty(name))
                        filename = name;
                }

                string logfilepath = Path.Combine(LogDirectory, filename);
                bool exists = File.Exists(logfilepath);
                entrycount = 0;
                writer = new StreamWriter(logfilepath);

                if (!exists)
                {
                    LogText("Log File Started on " + DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss"), LoggingLevel.Required);
                    LogText("Logging Levels: [D]iagnostic = 0, [I]nfo = 1, [W]arning = 2, [C]ritical = 3, [R]equired)", LoggingLevel.Required);
                }
            }
            catch (Exception ex)
            {
                LogCriticalError(ex);
            }
        }

        public void LogError(Exception ex, LoggingLevel level)
        {
            LogText("An error has occured", level);

            Exception e = ex;
            string spacer = "";

            lock (writer)
            {
                while (e != null)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine(spacer + "---------------------------------------------------------");
                    builder.AppendLine(spacer + String.Format("Error Message: {0}", e.Message));
                    builder.AppendLine(spacer + String.Format("Stack Trace: {0}", e.StackTrace));
                    builder.AppendLine(spacer + "---------------------------------------------------------");

                    writer.WriteLine(builder.ToString());
                    writer.Flush();
                    LogConsole(builder.ToString(), level);

                    e = e.InnerException;
                    spacer += "    ";
                }
            }
        }

        public void LogText(string message, LoggingLevel level)
        {
            if (writer == null)
                CreateLogFile();

            if (level < MinimumLoggingLevel)
                return;

            entrycount++;
            if (entrycount > MaxEntries && MaxEntries > 0)
            {
                CreateLogFile();
            }

            string dateformat = TimeFormat == LoggingTimeFormat.TwentyFourHour ? "ddd HH:mm:ss" : "ddd h:mm:ss tt";
            string text = string.Format("[{2}]{0}  {1}", DateTime.Now.ToString(dateformat), message, level.ToString().Substring(0, 1));
            lock (writer)
            {
                writer.WriteLine(text);
                writer.Flush();
            }

            LogConsole(text, level);
        }

        private void LogConsole(string text, LoggingLevel level)
        {
#if DEBUG
            ConsoleColor c = level == LoggingLevel.Diagnostic ? ConsoleColor.Magenta : level == LoggingLevel.Info ? ConsoleColor.DarkGreen : level == LoggingLevel.Warning ? ConsoleColor.DarkYellow : level == LoggingLevel.Required ? ConsoleColor.DarkCyan : ConsoleColor.DarkRed;
            Console.ForegroundColor = c;
            Console.WriteLine(text);
#endif
        }

        public void LogCriticalError(Exception ex)
        {
            LogCriticalError(ex.Message + System.Environment.NewLine + ex.StackTrace);
        }

        public void LogCriticalError(string text)
        {
            string sSource = "Application";
            string sLog = "FileAttachmentServer";
            string sEvent = text;

            if (!EventLog.SourceExists(sSource))
                EventLog.CreateEventSource(sSource, sLog);

            EventLog.WriteEntry(sSource, sEvent);
            EventLog.WriteEntry(sSource, sEvent, EventLogEntryType.Error);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (writer != null)
            {
                writer.Dispose();
                writer = null;
            }
        }
    }

    public enum LoggingLevel
    {
        Diagnostic = 0, Info = 1, Warning = 2, Critical = 3, Required = 100
    }

    public enum LoggingTimeFormat
    {
        TwelveHour = 0, TwentyFourHour = 1
    }

    public delegate string GetFileNameHandler(object sender);
}
