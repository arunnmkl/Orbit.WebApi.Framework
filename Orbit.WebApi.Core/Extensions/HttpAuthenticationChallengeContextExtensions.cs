using System;
using System.Net.Http.Headers;
using System.Web.Http.Filters;
using Orbit.WebApi.Core.Results;

namespace Orbit.WebApi.Core
{
    /// <summary>
    /// Class HttpAuthenticationChallengeContextExtensions.
    /// </summary>
    public static class HttpAuthenticationChallengeContextExtensions
    {
        /// <summary>
        /// Challenges the with.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="scheme">The scheme.</param>
        public static void ChallengeWith(this HttpAuthenticationChallengeContext context, string scheme)
        {
            ChallengeWith(context, new AuthenticationHeaderValue(scheme));
        }

        /// <summary>
        /// Challenges the with.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="scheme">The scheme.</param>
        /// <param name="parameter">The parameter.</param>
        public static void ChallengeWith(this HttpAuthenticationChallengeContext context, string scheme, string parameter)
        {
            ChallengeWith(context, new AuthenticationHeaderValue(scheme, parameter));
        }

        /// <summary>
        /// Challenges the with.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="challenge">The challenge.</param>
        /// <exception cref="System.ArgumentNullException">context</exception>
        public static void ChallengeWith(this HttpAuthenticationChallengeContext context, AuthenticationHeaderValue challenge)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.Result = new AddChallengeOnUnauthorizedResult(challenge, context.Result);
        }
    }
}