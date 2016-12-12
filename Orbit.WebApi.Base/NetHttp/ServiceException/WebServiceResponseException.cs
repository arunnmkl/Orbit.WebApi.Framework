using System;
using System.Text;

namespace Orbit.WebApi.Base.NetHttp.ServiceException
{
    /// <summary>
    /// Class WebServiceResponseException.
    /// </summary>
    public class WebServiceResponseException : Exception
    {
        /// <summary>
        /// Gets the type of the content.
        /// </summary>
        /// <value>The type of the content.</value>
        public string ContentType { get; private set; }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>The content.</value>
        public byte[] Content { get; private set; }

        /// <summary>
        /// Gets the content string.
        /// </summary>
        /// <value>The content string.</value>
        public string ContentString
        {
            get
            {
                try
                {
                    return Encoding.UTF8.GetString(Content, 0, Content.Length);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebServiceResponseException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="content">The content.</param>
        public WebServiceResponseException(string message, string contentType, byte[] content)
            : base(message)
        {
            ContentType = contentType;
            Content = content;
        }
    }
}