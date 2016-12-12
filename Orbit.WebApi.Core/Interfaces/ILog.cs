using System;

namespace Orbit.WebApi.Core.Interfaces
{
	/// <summary>
	/// Interface ILog
	/// </summary>
	public interface ILog
	{
		/// <summary>
		/// Logs the specified ex.
		/// </summary>
		/// <param name="ex">The ex.</param>
		void Log(Exception ex);
	}
}