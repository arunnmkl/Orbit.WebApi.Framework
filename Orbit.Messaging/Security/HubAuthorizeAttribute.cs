using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin.Security.DataHandler;
using Orbit.WebApi.Core.Security;

namespace Orbit.Messaging.Security
{
    /// <summary>
    /// Hub authorization attribute.
    /// </summary>
    /// <seealso cref="Microsoft.AspNet.SignalR.AuthorizeAttribute" />
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class HubAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Determines whether client is authorized to connect to <see cref="T:Microsoft.AspNet.SignalR.Hubs.IHub" />.
        /// </summary>
        /// <param name="hubDescriptor">Description of the hub client is attempting to connect to.</param>
        /// <param name="request">The (re)connect request from the client.</param>
        /// <returns>
        /// true if the caller is authorized to connect to the hub; otherwise, false.
        /// </returns>
        public override bool AuthorizeHubConnection(HubDescriptor hubDescriptor, IRequest request)
        {
            // Stub for code that does something non-trivial to come up with a custom principal.
            var context = request.GetHttpContext();

            string cookieToken = null,
                queryStringToken = null,
                headerToken = null;

            try
            {
                cookieToken = context.Request.Cookies[Configuration.Current.AuthCookieName].Value;
            }
            catch (NullReferenceException)
            {
                System.Diagnostics.Debug.WriteLine("The cookie does not contain the bearer token");
            }

            try
            {
                queryStringToken = context.Request.QueryString[Configuration.Current.AuthQueryStringName].ToString();
            }
            catch (NullReferenceException)
            {
                System.Diagnostics.Debug.WriteLine("The query string does not contain the bearer token");
            }

            try
            {
                var auth = context.Request.Headers["Authorization"];
                if (!string.IsNullOrEmpty(auth))
                {

                    AuthenticationHeaderValue authorization = AuthenticationHeaderValue.Parse(auth);
                    // If there are no authorization token in header, do nothing.
                    if (authorization != null)
                    {
                        // If there are authorization token but the filter does not recognize the 
                        // authentication scheme, do nothing.
                        if (authorization.Scheme.ToUpperInvariant() == Configuration.Current.AuthHeaderSchemaName.ToUpperInvariant())
                        {
                            headerToken = authorization.Parameter;
                        }
                    }
                }
            }
            catch (NullReferenceException)
            {
                System.Diagnostics.Debug.WriteLine("The connection header does not contain the bearer token");
            }

            if (context.User != null
                && (!string.IsNullOrEmpty(cookieToken)
                || !string.IsNullOrEmpty(headerToken)
                || !string.IsNullOrEmpty(queryStringToken)))
            {
                var protectedToken = headerToken ?? queryStringToken ?? cookieToken;
                var authTicket = new AccessTokenFormat().Unprotect(protectedToken);
                ApiPrincipal apiPrincipal = null;
                //var ticket = Startup.OAuthBearerOptions.AccessTokenFormat.Unprotect(protectedToken);
                if (authTicket != null)
                {
                    apiPrincipal = new ApiPrincipal(authTicket.Identity);
                }

                if (apiPrincipal != null)
                {
                    context.User = apiPrincipal;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return base.AuthorizeHubConnection(hubDescriptor, request);
        }

        /// <summary>
        /// Determines whether client is authorized to invoke the <see cref="T:Microsoft.AspNet.SignalR.Hubs.IHub" /> method.
        /// </summary>
        /// <param name="hubIncomingInvokerContext">An <see cref="T:Microsoft.AspNet.SignalR.Hubs.IHubIncomingInvokerContext" /> providing details regarding the <see cref="T:Microsoft.AspNet.SignalR.Hubs.IHub" /> method invocation.</param>
        /// <param name="appliesToMethod">Indicates whether the interface instance is an attribute applied directly to a method.</param>
        /// <returns>
        /// true if the caller is authorized to invoke the <see cref="T:Microsoft.AspNet.SignalR.Hubs.IHub" /> method; otherwise, false.
        /// </returns>
        public override bool AuthorizeHubMethodInvocation(IHubIncomingInvokerContext hubIncomingInvokerContext, bool appliesToMethod)
        {
            if (SkipAuthorization(hubIncomingInvokerContext.MethodDescriptor))
            {
                return true;
            }

            return base.AuthorizeHubMethodInvocation(hubIncomingInvokerContext, appliesToMethod);
        }

        /// <summary>
        /// When overridden, provides an entry point for custom authorization checks.
        /// Called by <see cref="M:Microsoft.AspNet.SignalR.AuthorizeAttribute.AuthorizeHubConnection(Microsoft.AspNet.SignalR.Hubs.HubDescriptor,Microsoft.AspNet.SignalR.IRequest)" /> and <see cref="M:Microsoft.AspNet.SignalR.AuthorizeAttribute.AuthorizeHubMethodInvocation(Microsoft.AspNet.SignalR.Hubs.IHubIncomingInvokerContext,System.Boolean)" />.
        /// </summary>
        /// <param name="user">The <see cref="T:System.Security.Principal.IPrincipal" /> for the client being authorize</param>
        /// <returns>
        /// true if the user is authorized, otherwise, false
        /// </returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        protected override bool UserAuthorized(IPrincipal user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var principal = new ApiPrincipal(user as ClaimsPrincipal);

            if (principal != null)
            {
                var userId = principal.UserId;
                if (userId != default(long))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Skips the authorization.
        /// </summary>
        /// <param name="methodDescriptor">The method descriptor.</param>
        /// <returns>the authorization</returns>
        private bool SkipAuthorization(MethodDescriptor methodDescriptor)
        {
            var isAnonymous = methodDescriptor.Attributes.OfType<System.Web.Http.AllowAnonymousAttribute>();

            if (isAnonymous.Any())
            {
                return true;
            }

            return false;
        }
    }
}