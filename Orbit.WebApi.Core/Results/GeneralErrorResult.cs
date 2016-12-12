using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Orbit.WebApi.Core.Results
{
    /// <summary>
    /// Class GeneralErrorResult.
    /// </summary>
    public class GeneralErrorResult : IHttpActionResult
    {
        /// <summary>
        /// The response message
        /// </summary>
        private object responseMessage;

        /// <summary>
        /// Gets or sets the request.
        /// </summary>
        /// <value>The request.</value>
        public HttpRequestMessage Request { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public string Content { get; set; }

        /// <summary>
        /// Gets the reason phrase.
        /// </summary>
        /// <value>
        /// The reason phrase.
        /// </value>
        public string ReasonPhrase { get; private set; }

        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        /// <value>
        /// The status code.
        /// </value>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralErrorResult"/> class.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="content">The content.</param>
        public GeneralErrorResult(HttpRequestMessage request, string content) : this("Internal Server Error", request, content, HttpStatusCode.InternalServerError, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralErrorResult" /> class.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="content">The content.</param>
        /// <param name="responseMessage">The response message.</param>
        public GeneralErrorResult(HttpRequestMessage request, string content, object responseMessage) : this("Internal Server Error", request, content, HttpStatusCode.InternalServerError, responseMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralErrorResult" /> class.
        /// </summary>
        /// <param name="reasonPhrase">The reason phrase.</param>
        /// <param name="request">The request.</param>
        /// <param name="content">The content.</param>
        /// <param name="statusCode">The status code.</param>
        public GeneralErrorResult(string reasonPhrase, HttpRequestMessage request, string content, HttpStatusCode statusCode) : this(reasonPhrase, request, content, statusCode, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralErrorResult" /> class.
        /// </summary>
        /// <param name="reasonPhrase">The reason phrase.</param>
        /// <param name="request">The request.</param>
        /// <param name="content">The content.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="responseMessage">The response message.</param>
        public GeneralErrorResult(string reasonPhrase, HttpRequestMessage request, string content, HttpStatusCode statusCode, object responseMessage)
        {
            ReasonPhrase = reasonPhrase;
            Request = request;
            Content = content;
            StatusCode = statusCode;
            this.responseMessage = responseMessage;
        }

        /// <summary>
        /// Creates an <see cref="T:System.Net.Http.HttpResponseMessage" /> asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that, when completed, contains the <see cref="T:System.Net.Http.HttpResponseMessage" />.</returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = Request.CreateResponse(StatusCode, Content);
            if (responseMessage != null)
            {
                MediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();
                response.Content = new ObjectContent<object>(responseMessage, jsonFormatter);
            }
            response.RequestMessage = Request;
            response.ReasonPhrase = ReasonPhrase;
            return Task.FromResult(response);
        }
    }
}