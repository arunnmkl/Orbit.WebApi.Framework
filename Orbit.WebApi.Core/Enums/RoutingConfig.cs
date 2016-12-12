namespace Orbit.WebApi.Core.Enums
{
    /// <summary>
    /// This is an ENUM to introduce the configuration for the web route creation
    /// </summary>
    /// <see cref="Orbit.WebApi.Core.Config" />
    public enum RoutingConfig
    {
        /// <summary>
        /// THis is for the default route creation.
        /// </summary>
        Default,

        /// <summary>
        /// This is used to resolve the same ApiContoller with same but in different namespace.
        /// </summary>
        /// <example>V1.CustomerController and V2.CustomerController</example>
        Namespace
    }
}