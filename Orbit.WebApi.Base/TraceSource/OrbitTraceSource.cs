using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace Orbit.WebApi.Base.TraceSource
{
    public static class OrbitTraceSource
    {
        /// <summary>
        /// The trace source in from the given config
        /// </summary>
        private static readonly System.Diagnostics.TraceSource OrbitSource = new System.Diagnostics.TraceSource("OrbitTraceSource");

        /// <summary>
        /// Writes the specified message to the trace
        /// </summary>
        /// <param name="level">The severity level of the message</param>
        /// <param name="message">The message.</param>
        /// <param name="callingAssembly">The assembly that should be recorded as the source of the event</param>
        /// <param name="exception">The corresponding Exception that occurred</param>
        /// <param name="categories">The categories.</param>
        public static void Write(TraceEventType level, string message, Assembly callingAssembly = null, Exception exception = null, params string[] categories)
        {
            string userName = null;

            if (Thread.CurrentPrincipal != null)
            {
                userName = Thread.CurrentPrincipal.Identity.Name;
            }

            var entry = new LogEntry
            {
                Severity = level,
                MachineName = Environment.MachineName,
                AssemblyFullName = (callingAssembly ?? Assembly.GetCallingAssembly()).FullName,
                Message = message,
                Exception = exception,
                Categories = categories,
                UserName = userName,
            };

            Write(entry);
        }

        /// <summary>
        /// Writes the specified log entry to the trace
        /// </summary>
        /// <param name="logEntry">The log entry to write</param>
        public static void Write(LogEntry logEntry)
        {
            OrbitSource.TraceData(logEntry.Severity, 0, logEntry);
        }

        /// <summary>
        /// Writes the message to the trace with severity level 'Information'
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="categories">The categories.</param>
        public static void Info(string message, params string[] categories)
        {
            Write(TraceEventType.Information, message, Assembly.GetCallingAssembly(), categories: categories);
        }

        /// <summary>
        /// Writes the message to the trace with severity level 'Verbose'
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="categories">The categories.</param>
        public static void Verbose(string message, params string[] categories)
        {
            Write(TraceEventType.Verbose, message, Assembly.GetCallingAssembly(), categories: categories);
        }

        /// <summary>
        /// Writes the message to the trace with severity level 'Warning'
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="categories">The categories.</param>
        public static void Warning(string message, params string[] categories)
        {
            Write(TraceEventType.Warning, message, Assembly.GetCallingAssembly(), categories: categories);
        }

        /// <summary>
        /// Writes the message to the trace with severity level 'Warning';
        /// The string representation of the exception is included in the message
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="categories">The categories.</param>
        /// <param name="ex">The exception which inspired this warning</param>
        public static void Warning(string message, Exception ex, params string[] categories)
        {
            Write(TraceEventType.Warning, message, Assembly.GetCallingAssembly(), ex, categories);
        }

        /// <summary>
        /// Writes the message to the trace with severity level 'Error';
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="categories">The categories.</param>
        public static void Error(string message, params string[] categories)
        {
            Write(TraceEventType.Error, message, Assembly.GetCallingAssembly(), categories: categories);
        }

        /// <summary>
        /// Writes the message to the trace with severity level 'Error';
        /// The string representation of the exception is included in the message
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="categories">The categories.</param>
        /// <param name="ex">The exception which inspired this error</param>
        public static void Error(string message, Exception ex, params string[] categories)
        {
            Write(TraceEventType.Error, message, Assembly.GetCallingAssembly(), ex, categories);
        }
    }
}