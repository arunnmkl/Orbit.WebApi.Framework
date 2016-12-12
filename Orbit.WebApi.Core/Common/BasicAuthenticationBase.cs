using System;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using Orbit.WebApi.Core.Interfaces;

namespace Orbit.WebApi.Core.Common
{
    /// <summary>
    /// Class Basic AuthenticationBase with the implementation of SkipAuthorizationBase.
    /// </summary>
    public abstract class BasicAuthenticationBase : SkipAuthorizationBase, IBasicAuthentication
    {
        /// <summary>
        /// Gets or sets the error message.
        /// This value should be get populated once any error has come so that User will get to know the exact error in the response
        /// </summary>
        /// <value>The error message.</value>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Authenticates the token.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="context"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// true whether the token is valid else return false.
        /// </returns>
        public abstract Task<bool> AuthenticateToken(string accessToken, HttpAuthenticationContext context, CancellationToken cancellationToken);

        /// <summary>
        /// Authenticates the username password.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns>IPrincipal.</returns>
        public abstract IPrincipal AuthenticateUsernamePassword(string userName, string password);

        /// <summary>
        /// Extracts the user name and password.
        /// </summary>
        /// <param name="authorizationParameter">The authorization parameter.</param>
        /// <returns>In the form of username and password</returns>
        public virtual Tuple<string, string> ExtractUserNameAndPassword(string authorizationParameter)
        {
            byte[] credentialBytes;

            try
            {
                credentialBytes = Convert.FromBase64String(authorizationParameter);
            }
            catch (FormatException)
            {
                return null;
            }

            // The currently approved HTTP 1.1 specification says characters here are ISO-8859-1.
            // However, the current draft updated specification for HTTP 1.1 indicates this encoding is infrequently
            // used in practice and defines behavior only for ASCII.
            Encoding encoding = Encoding.ASCII;

            // Make a writable copy of the encoding to enable setting a decoder fall back.
            encoding = (Encoding)encoding.Clone();

            // Fail on invalid bytes rather than silently replacing and continuing.
            encoding.DecoderFallback = DecoderFallback.ExceptionFallback;
            string decodedCredentials;

            try
            {
                decodedCredentials = encoding.GetString(credentialBytes);
            }
            catch (DecoderFallbackException)
            {
                return null;
            }

            if (String.IsNullOrEmpty(decodedCredentials))
            {
                return null;
            }

            int colonIndex = decodedCredentials.IndexOf(':');
            if (colonIndex == -1)
            {
                return null;
            }

            string userName = decodedCredentials.Substring(0, colonIndex);
            string password = decodedCredentials.Substring(colonIndex + 1);
            return new Tuple<string, string>(userName, password);
        }

        /// <summary>
        /// authenticate as an asynchronous operation.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task IPrincipal.</returns>
        public async Task<IPrincipal> AuthenticateAsync(System.Web.Http.Filters.HttpAuthenticationContext context, System.Threading.CancellationToken cancellationToken)
        {
            var request = context.Request;
            var authenticationHeader = request.Headers.Authorization;

            if (authenticationHeader == null
                || authenticationHeader.Scheme != "Basic"
                || String.IsNullOrEmpty(authenticationHeader.Parameter))
            {
                // No authentication was attempted (for this authentication method).
                // Do not set either Principal (which would indicate success) or ErrorResult (indicating an error).
                return null;
            }

            Tuple<string, string> userNameAndPassword = ExtractUserNameAndPassword(authenticationHeader.Parameter);

            if (userNameAndPassword == null)
            {
                // Authentication was attempted but failed. Set ErrorResult to indicate an error.
                ErrorMessage = "Invalid credentials";
                return null;
            }

            var userName = userNameAndPassword.Item1;
            var password = userNameAndPassword.Item2;

            IPrincipal principal = await Task.Run(() =>
            {
                return AuthenticateUsernamePassword(userName, password);
            });

            if (principal != null)
            {
                context.ActionContext.Response.SetAuthentication("SecureTicketString");
            }

            return principal;
        }

        /// <summary>
        /// Authenticates the specified HTTP request base.
        /// </summary>
        /// <param name="httpRequestBase">The HTTP request base.</param>
        /// <returns>IPrincipal.</returns>
        public IPrincipal Authenticate(HttpRequestBase httpRequestBase)
        {
            string authorization = httpRequestBase.Headers["Authorization"];
            string errorMessage = string.Empty;

            if (!string.IsNullOrEmpty(authorization))
            {
                try
                {
                    var credentials = ExtractUserNameAndPassword(authorization.Substring(6));

                    IPrincipal auth = AuthenticateUsernamePassword(credentials.Item1, credentials.Item2);

                    return auth;
                }
                catch (Exception ex)
                {
                    errorMessage = ex.ToString();
                }
            }

            return null;
        }

        /// <summary>
        /// Sets the authentication value to the response for the basic auth.
        /// </summary>
        /// <param name="principalTicket">The principal ticket.</param>
        /// <param name="response">The response.</param>
        public virtual void SetAuthentication(string principalTicket, HttpResponseMessage response)
        {
            response.SetAuthentication(principalTicket);
        }
    }
}