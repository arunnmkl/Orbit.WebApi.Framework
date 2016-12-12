using System.Web.Http;
using System.Web.Http.Dispatcher;
using Orbit.WebApi.Core.Enums;
using Orbit.WebApi.Core.Security;
using Orbit.WebApi.Core.Interfaces;
using Orbit.WebApi.Core.Exceptions;
using Orbit.WebApi.Core.Dependency;

/// <summary>
/// This namespace contains all the extension methods which are used by Orbit.WebApi project.
/// </summary>
namespace Orbit.WebApi.Core
{
	/// <summary>
	/// THis is used to configure the API configuration based on the requirement.
	/// </summary>
	/// <see cref="System.Web.Http.HttpConfiguration"/> 
	public static class Config
	{
		/// <summary>
		/// Gets the route configuration.
		/// </summary>
		/// <value>
		/// The route configuration.
		/// </value>
		public static string ApiRouteVersion
		{
			get
			{
				if (!string.IsNullOrEmpty(Configuration.Current.ApiVersion))
				{
					return string.Concat("API", string.Format("/{0}/", Configuration.Current.ApiVersion));
				}

				return string.Concat("API/");
			}
		}

		/// <summary>
		/// Configures the specified routing configuration.
		/// </summary>
		/// <param name="config">The configuration.</param>
		/// <param name="routingConfig">The routing configuration.</param>
		public static void Configure(this HttpConfiguration config, RoutingConfig routingConfig = RoutingConfig.Default)
		{
			ConfigureByRoutingConfig(config, routingConfig);
		}

		/// <summary>
		/// Validates the specified configuration. call this method after all the configuration is done.
		/// </summary>
		/// <param name="config">The configuration.</param>
		public static void Validate(this HttpConfiguration config)
		{
			if (Configuration.Current.AuthenticationEnabled)
			{
				if (!DependencyResolverContainer.IsValid<IAuthenticationCommand>())
					throw new WebApiException("Please add dependency for the IAuthenticationCommand, or if don't what CSRF feature then mark enableAuthentication as false in configuration.");
			}

			if (Configuration.Current.CSRFAttackPrevented)
			{
				if (!DependencyResolverContainer.IsValid<ICSRFValidation>())
					throw new WebApiException("Please add dependency for the ICSRFValidation, or if don't what CSRF feature then mark preventCSRFAttack as false in configuration.");
			}
		}

		/// <summary>
		/// Configures HttpConfiguration by the given routing configuration.
		/// </summary>
		/// <param name="config">The configuration.</param>
		/// <param name="routingConfig">The routing configuration.</param>
		private static void ConfigureByRoutingConfig(HttpConfiguration config, RoutingConfig routingConfig)
		{
			switch (routingConfig)
			{
				case RoutingConfig.Default:
					config.Routes.MapHttpRoute(name: "Orbit Default API",
										routeTemplate: string.Concat(ApiRouteVersion, "{controller}/{id}"),
										defaults: new { id = RouteParameter.Optional });
					break;

				case RoutingConfig.Namespace:
					config.Routes.MapHttpRoute(name: "Orbit Namespace API",
										routeTemplate: string.Concat(ApiRouteVersion, "{namespace}/{controller}/{id}"),
										defaults: new { id = RouteParameter.Optional });

					config.Services.Replace(typeof(IHttpControllerSelector), new NamespaceHttpControllerSelector(config));
					break;

				default:
					break;
			}
		}
	}
}