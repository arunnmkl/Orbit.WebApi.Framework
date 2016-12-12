using System;
using System.Diagnostics;

namespace Orbit.WebApi.Base.TraceSource
{
    public class LogEntry
    {
        public TraceEventType Severity;
        public DateTime EventTime = DateTime.Now;
        public string MachineName = Environment.MachineName;
        public string AssemblyFullName = "";
        public string Message;
        public Exception Exception;
        public string[] Categories;
        public string Source;
        public string UserName;

        public override string ToString()
        {
            return String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}",
                String.Join(":", Categories ?? new string[] { "None" }),
                Severity,
                EventTime,
                MachineName ?? "",
                AssemblyFullName ?? "",
                Message ?? "",
                Exception ?? new Exception("AKK"),
                UserName ?? "");
        }
    }
}
