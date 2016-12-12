using System;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using Orbit.WebApi.Core;
using Orbit.WebApi.Core.Common;
using Orbit.WebApi.Core.Interfaces;
using Orbit.WebApi.Extensions.Validation;
using ApiSecurity = Orbit.WebApi.Core.Security;

namespace Orbit.WebApi.Extensions.Authentication
{
    /// <summary>
    /// Class class CookieAuthenticationController.
    /// </summary>
    public class CookieAuthenticationController : SkipAuthorizationBase, ICookieAuthentication
    {
        /// <summary>
        /// Gets or sets the error/Exception message.
        /// </summary>
        /// <value>The error message.</value>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Authenticates the cookie.
        /// </summary>
        /// <param name="ticket">The ticket.</param>
        /// <returns>IPrincipal.</returns>
        public IPrincipal AuthenticateCookie(string ticket)
        {
            IPrincipal principal = null;

            try
            {
                if (!string.IsNullOrEmpty(ticket))
                {
                    var claimsIdentity = AuthenticationCommands.ConvertTokenAsClaimsIdentity(ticket);
                    if (claimsIdentity != null)
                    {
                        principal = new ApiSecurity.ApiPrincipal(claimsIdentity);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return principal;
        }

        /// <summary>
        /// Authenticates the asynchronous.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;IPrincipal&gt;.</returns>
        public async Task<IPrincipal> AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            IPrincipal principal = null;
            HttpRequestMessage request = context.Request;
            string authCookie = request.GetCookie(ApiSecurity.Configuration.Current.AuthCookieName);
            string authToken = request.GetQueryString(ApiSecurity.Configuration.Current.AuthCookieName);

            string ticket = authCookie != null ? authCookie : null;
            if (ticket == null && !string.IsNullOrWhiteSpace(authToken))
            {
                ticket = Uri.UnescapeDataString(authToken);
            }

            if (string.IsNullOrEmpty(ticket))
            {
                ErrorMessage = "Invalid credentials.";
                return principal;
            }

            bool isValid = await Task.Run(() => { return AuthenticateToken(ticket, context, cancellationToken); });

            if (isValid)
            {
                principal = await Task.Run(() =>
                {
                    return AuthenticateCookie(ticket);
                });
            }

            return principal;
        }

        /// <summary>
        /// Authenticates the specified HTTP request base. This is used in the MVC applications.
        /// </summary>
        /// <param name="httpRequestBase">The HTTP request base.</param>
        /// <returns>The implementation of the current principle</returns>
        public IPrincipal Authenticate(HttpRequestBase httpRequestBase)
        {
            var value = httpRequestBase.Cookies[ApiSecurity.Configuration.Current.AuthCookieName];

            if (value != null)
            {
                string ticket = ticket = Uri.UnescapeDataString(value.Value);

                var principal = AuthenticateCookie(ticket);

                return principal;
            }

            return null;
        }

        /// <summary>
        /// Authenticates the token.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="context"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// true whether the token is valid else return false.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> AuthenticateToken(string accessToken, HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var authResult = await AuthenticationTokenValidator.AuthenticateToken(accessToken, context, cancellationToken);
            if (authResult.ErrorResponse != null)
            {
                ErrorMessage = authResult.ErrorResponse.Error.Message;
            }

            return authResult.IsValid;
        }
    }
}