using System;

namespace Orbit.WebApi.Core.Exceptions
{
	[Serializable]
	public class WebApiException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="WebApiException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="exception">The exception.</param>
		public WebApiException(string message, Exception exception)
			: base(message, exception)
		{
		}

		public WebApiException(string message)
			: base(message)
		{
		}
	}
}