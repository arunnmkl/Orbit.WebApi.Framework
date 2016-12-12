using Orbit.WebApi.Extensions.Validation;

namespace Orbit.WebApi.Extensions.Owin.Externals
{
    /// <summary>
    /// This is a factory class which gives the instance of the providers.
    /// </summary>
    public static class ExternalValidationFactory
    {
        /// <summary>
        /// Gets the external validation.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static IExternalValidation GetExternalValidation(string provider)
        {
            switch (provider)
            {
                case "Facebook":
                    return new FacebookValidation();

                case "Google":
                    return new GoogleValidation();

                default:
                    return null;
            }
        }
    }
}