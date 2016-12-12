using System;
using Orbit.WebApi.Base.TraceSource;
using Orbit.WebApi.Core.Interfaces;

namespace Orbit.WebApi.Extensions.Logging
{
    /// <summary>
    /// This class is used for logging
    /// </summary>
    public class Logging : ILog
    {
        /// <summary>
        /// Logs the specified exception.
        /// </summary>
        /// <param name="ex">The exception.</param>
        public void Log(Exception ex)
        {
            try
            {
                OrbitTraceSource.Error(ex.Message, ex, new string[] { "Error" });
            }
            catch (Exception)
            {
            }
        }
    }
}