using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace csgoDL
{
    /// <summary>
    /// USed to write information about not handled exceptions.
    /// </summary>
    class UnhandledExceptionHandler
    {
        /// <summary>
        /// The name of the log file to write to.
        /// </summary>
        public const string CRASHLOG_FILENAME = "csgoDL_crashlog.txt";
        /// <summary>
        /// The log action to use.
        /// </summary>
        public Action<string> LogAction { get; set; }
        /// <summary>
        /// Passes the line to the outer logging action.
        /// </summary>
        /// <param name="line">The line to pass.</param>
        private void LogToOuter(string line) { LogAction?.Invoke(line); }
        /// <summary>
        /// Logs an unhandled exception.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The exception to log.</param>
        [HandleProcessCorruptedStateExceptions]
        public void LogUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Log it
            LogException((Exception)e.ExceptionObject);
        }
        /// <summary>
        /// Logs an unhandled exception.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The exception to log.</param>
        public void LogUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Log it
            LogException(e.Exception);
        }
        /// <summary>
        /// Logs an unhandled exception.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The exception to log.</param>
        public void LogUnhandledException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            // Log it
            LogException(e.Exception);
        }
        /// <summary>
        /// Logs the given exception.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        private void LogException(Exception ex)
        {
            using (StreamWriter sw = new StreamWriter(CRASHLOG_FILENAME, true))
            {
                Action<string> logLine = (string msg) => { sw.WriteLine(msg); LogToOuter(msg); };
                logLine("-----------------------------------------------------------");
                logLine("!!! Caught an unhandled exception: " + ex.Message + " timestamp: " + DateTime.Now.ToString(CultureInfo.InvariantCulture));
                logLine("-----------------------------------------------------------");
                logLine("Time: " + DateTime.Now.ToString());
                logLine("Stacktrace:");
                logLine(ex.StackTrace);
                logLine("InnerException: ");
                if (ex.InnerException != null)
                {
                    logLine(ex.InnerException.Message);
                    logLine("Stacktrace:");
                    logLine(ex.InnerException.StackTrace);
                }
                else
                {
                    logLine("None");
                }
                logLine("-----------------------------------------------------------");
            }
        }
    }
}
