using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Orbit.WebApi.Core.Dependency;
using Orbit.WebApi.Core.Interfaces;

namespace Orbit.WebApi.Core.Handlers
{
    /// <summary>
    /// Class ApiKeyHandler.
    /// </summary>
    public class ApiKeyHandler : DelegatingHandler
    {
        /// <summary>
        /// The _key validator
        /// </summary>
        private IApiKeyValidator _keyValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiKeyHandler"/> class.
        /// </summary>
        public ApiKeyHandler()
        {
            this._keyValidator = DependencyResolverContainer.Resolve<IApiKeyValidator>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiKeyHandler"/> class.
        /// </summary>
        /// <param name="keyValidator">The key validator.</param>
        public ApiKeyHandler(IApiKeyValidator keyValidator)
        {
            this._keyValidator = keyValidator;
        }

        /// <summary>
        /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />. The task object representing the asynchronous operation.</returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!_keyValidator.ValidateKey(request))
            {
                var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                var tsc = new TaskCompletionSource<HttpResponseMessage>();
                tsc.SetResult(response);
                return tsc.Task;
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}