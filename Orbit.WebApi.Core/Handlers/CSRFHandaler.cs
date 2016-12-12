using Orbit.WebApi.Core.Security;
using Orbit.WebApi.Core.Interfaces;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Orbit.WebApi.Core.Dependency;

namespace Orbit.WebApi.Core.Handlers
{
    /// <summary>
    /// A handler which handles the CSRF attack and adds the values to the cookie
    /// </summary>
    public class CSRFHandler : DelegatingHandler
    {
        /// <summary>
        /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />. The task object representing the asynchronous operation.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            // throw the exception if requested, cancel the the request
            cancellationToken.ThrowIfCancellationRequested();

            if (response.IsSuccessStatusCode && Configuration.Current.CSRFAttackPrevented)
            {
                ICSRFValidation validator = DependencyResolverContainer.Resolve<ICSRFValidation>();
                if (validator != null)
                {
                    string csrfNumber = validator.GetCSRFValue();
                    CookieHeaderValue csrfCookie = new CookieHeaderValue(Configuration.Current.CSRFCookieName, csrfNumber);
                    csrfCookie.Path = "/";
                    response.Headers.AddCookies(new CookieHeaderValue[] { csrfCookie });
                }
            }

            return response;
        }
    }
}