using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;

namespace Orbit.WebApi.Core
{
    /// <summary>
    /// Namespace http controller selector, as custom controller selector
    /// </summary>
    public class NamespaceHttpControllerSelector : IHttpControllerSelector
    {
        #region Private members

        /// <summary>
        /// Stores the namespace key name
        /// </summary>
        private const string NamespaceKey = "namespace";

        /// <summary>
        /// Stores the controller key name
        /// </summary>
        private const string ControllerKey = "controller";

        /// <summary>
        /// Stores the http configuration
        /// </summary>
        private readonly HttpConfiguration _configuration;

        /// <summary>
        /// Stores all the controllers
        /// </summary>
        private readonly Lazy<Dictionary<string, HttpControllerDescriptor>> _controllers;

        /// <summary>
        /// Stores the duplicates controllers
        /// </summary>
        private readonly HashSet<string> _duplicates;

        #endregion Private members

        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceHttpControllerSelector" /> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public NamespaceHttpControllerSelector(HttpConfiguration config)
        {
            _configuration = config;
            _duplicates = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _controllers = new Lazy<Dictionary<string, HttpControllerDescriptor>>(InitializeControllerDictionary);
        }

        #region IHttpControllerSelector

        /// <summary>
        /// Returns a map, keyed by controller string, of all <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor" /> that the selector can select.  This is primarily called by <see cref="T:System.Web.Http.Description.IApiExplorer" /> to discover all the possible controllers in the system.
        /// </summary>
        /// <returns>A map of all <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor" /> that the selector can select, or null if the selector does not have a well-defined mapping of <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor" />.</returns>
        public IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
        {
            return _controllers.Value;
        }

        /// <summary>
        /// Selects a <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor" /> for the given <see cref="T:System.Net.Http.HttpRequestMessage" />.
        /// </summary>
        /// <param name="request">The request message.</param>
        /// <returns>An <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor" /> instance.</returns>
        /// <exception cref="System.Web.Http.HttpResponseException">
        /// </exception>
        public HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            IHttpRouteData routeData = request.GetRouteData();
            if (routeData == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            // Get the namespace and controller variables from the route data.
            string namespaceName = GetRouteVariable<string>(routeData, NamespaceKey);
            if (namespaceName == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            string controllerName = GetRouteVariable<string>(routeData, ControllerKey);
            if (controllerName == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            // Find a matching controller.
            string key = String.Format(CultureInfo.InvariantCulture, "{0}/{1}", namespaceName, controllerName);

            HttpControllerDescriptor controllerDescriptor;
            if (_controllers.Value.TryGetValue(key, out controllerDescriptor))
            {
                return controllerDescriptor;
            }
            else if (_duplicates.Contains(key))
            {
                throw new HttpResponseException(
                    request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    "Multiple controllers were found that match this request."));
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        #endregion IHttpControllerSelector

        #region Private members

        /// <summary>
        /// Get a value from the route data, if present.
        /// </summary>
        /// <typeparam name="T">type of the value</typeparam>
        /// <param name="routeData">The route data.</param>
        /// <param name="name">The name.</param>
        /// <returns>default of T or the value</returns>
        private static T GetRouteVariable<T>(IHttpRouteData routeData, string name)
        {
            object result = null;
            if (routeData.Values.TryGetValue(name, out result))
            {
                return (T)result;
            }
            return default(T);
        }

        /// <summary>
        /// Initializes the controller dictionary.
        /// </summary>
        /// <returns>Dictionary&lt;System.String, HttpControllerDescriptor&gt;.</returns>
        private Dictionary<string, HttpControllerDescriptor> InitializeControllerDictionary()
        {
            var dictionary = new Dictionary<string, HttpControllerDescriptor>(StringComparer.OrdinalIgnoreCase);

            // Create a lookup table where key is "namespace.controller". The value of "namespace" is the last
            // segment of the full namespace. For example:
            // MyApplication.Controllers.V1.ProductsController => "V1.Products"
            IAssembliesResolver assembliesResolver = _configuration.Services.GetAssembliesResolver();
            IHttpControllerTypeResolver controllersResolver = _configuration.Services.GetHttpControllerTypeResolver();

            ICollection<Type> controllerTypes = controllersResolver.GetControllerTypes(assembliesResolver);

            foreach (Type controllerType in controllerTypes)
            {
                string[] segments = controllerType.Namespace.Split(Type.Delimiter);

                // For the dictionary key, strip "Controller" from the end of the type name.
                // This matches the behavior of DefaultHttpControllerSelector.
                string controllerName = controllerType.Name.Remove(controllerType.Name.Length - DefaultHttpControllerSelector.ControllerSuffix.Length);

                string namespaceName = segments[segments.Length - 1];

                string key = String.Format(CultureInfo.InvariantCulture, "{0}/{1}", namespaceName, controllerName);

                // Check for duplicate keys.
                if (dictionary.Keys.Contains(key))
                {
                    _duplicates.Add(key);
                }
                else
                {
                    dictionary[key] = new HttpControllerDescriptor(_configuration, string.Format("{0} : {1}", namespaceName, controllerName), controllerType);
                }
            }

            // Remove any duplicates from the dictionary, because these create ambiguous matches.
            // For example, "Foo.V1.ProductsController" and "Bar.V1.ProductsController" both map to "v1.products".
            foreach (string s in _duplicates)
            {
                dictionary.Remove(s);
            }
            return dictionary;
        }

        #endregion Private members
    }
}