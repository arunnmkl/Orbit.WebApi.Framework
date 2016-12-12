using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbit.WebApi.Base.TraceSource
{
    public abstract class OrbitTraceListener : TraceListener
    {
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            var logEntry = data as LogEntry;

            if (logEntry != null && logEntry.Source == null)
            {
                logEntry.Source = source;
            }

            Write(data);
        }

        #region WriteLine Implementations

        public override void WriteLine(string message)
        {
            Write(message);
        }

        public override void WriteLine(string message, string category)
        {
            Write(message, category);
        }

        public override void WriteLine(object o)
        {
            Write(o);
        }

        public override void WriteLine(object o, string category)
        {
            Write(o, category);
        }

        #endregion WriteLine Implementations

        #region Write Overloads

        public override void Write(string message)
        {
            Write(new LogEntry { Message = message });
        }

        public override void Write(string message, string category)
        {
            Write(new LogEntry { Message = message, Categories = new[] { category } });
        }

        public override void Write(object o, string category)
        {
            if (o is LogEntry)
            {
                var entry = (LogEntry)o;

                if (entry.Categories != null)
                {
                    var categories = entry.Categories.ToList();
                    categories.Add(category);
                    entry.Categories = categories.ToArray();
                }
                else
                {
                    entry.Categories = new[] { category };
                }

                Write(entry);
            }
            else
            {
                Write(o.ToString(), category);
            }
        }

        #endregion Write Overloads

        public abstract void Write(object o);
    }
}