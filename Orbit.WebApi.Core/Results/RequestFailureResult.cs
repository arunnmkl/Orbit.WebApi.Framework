using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Orbit.WebApi.Core.Results
{
    /// <summary>
    /// Class RequestFailureResult.
    /// </summary>
    public class RequestFailureResult : IHttpActionResult
    {
        /// <summary>
        /// Gets the reason phrase.
        /// </summary>
        /// <value>The reason phrase.</value>
        public string ReasonPhrase { get; private set; }

        /// <summary>
        /// Gets the request.
        /// </summary>
        /// <value>The request.</value>
        public HttpRequestMessage Request { get; private set; }

        /// <summary>
        /// Gets or sets the HTTP status code.
        /// </summary>
        /// <value>The HTTP status code.</value>
        public HttpStatusCode HttpStatusCode { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestFailureResult" /> class.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="reasonPhrase">The reason phrase.</param>
        public RequestFailureResult(HttpRequestMessage request, HttpStatusCode statusCode, string reasonPhrase = "")
        {
            HttpStatusCode = statusCode;
            this.ReasonPhrase = reasonPhrase;
            this.Request = request;
        }

        /// <summary>
        /// Creates an <see cref="T:System.Net.Http.HttpResponseMessage" /> asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that, when completed, contains the <see cref="T:System.Net.Http.HttpResponseMessage" />.</returns>
        public Task<System.Net.Http.HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns>This is the http response for the unauthorized request.</returns>
        private HttpResponseMessage Execute()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode);
            response.RequestMessage = Request;
            response.ReasonPhrase = ReasonPhrase;
            return response;
        }
    }
}
