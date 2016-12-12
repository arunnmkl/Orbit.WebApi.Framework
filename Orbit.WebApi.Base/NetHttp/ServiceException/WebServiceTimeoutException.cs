namespace Orbit.WebApi.Base.NetHttp.ServiceException
{
    /// <summary>
    /// Class WebServiceTimeoutException.
    /// </summary>
    public class WebServiceTimeoutException : WebServiceRequestException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebServiceTimeoutException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="verb">The verb.</param>
        /// <param name="uri">The URI.</param>
        public WebServiceTimeoutException(string message, string verb, string uri)
            : base(message, verb, uri)
        {
        }
    }
}