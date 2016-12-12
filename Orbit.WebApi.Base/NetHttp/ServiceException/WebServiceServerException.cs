using System;

namespace Orbit.WebApi.Base.NetHttp.ServiceException
{
    /// <summary>
    /// Class WebServiceServerException.
    /// </summary>
    public class WebServiceServerException : Exception
    {
        /// <summary>
        /// Gets the error code.
        /// </summary>
        /// <value>The error code.</value>
        public string ErrorCode { get; private set; }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>The content.</value>
        public string Content { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebServiceServerException"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="innerException">The inner exception.</param>
        public WebServiceServerException(string errorCode, Exception innerException)
            : base(errorCode, innerException)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebServiceServerException"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="content">The content.</param>
        /// <param name="innerException">The inner exception.</param>
        public WebServiceServerException(string errorCode, string content, Exception innerException)
            : this(errorCode, innerException)
        {
            Content = content;
        }
    }
}