using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;

namespace Orbit.WebApi.Core.Dependency
{
    /// <summary>
    /// This class is responsible for resolving all the dependencies in the     api.
    /// </summary>
    public sealed class DependencyResolver : IDependencyResolver
    {
        /// <summary>
        /// Starts a resolution scope.
        /// </summary>
        /// <returns>The dependency scope.</returns>
        public IDependencyScope BeginScope()
        {
            return new DependencyResolver();
        }

        /// <summary>
        /// Retrieves a service from the scope.
        /// </summary>
        /// <param name="serviceType">The service to be retrieved.</param>
        /// <returns>The retrieved service.</returns>
        public object GetService(Type serviceType)
        {
            return DependencyResolverContainer.Resolve(serviceType);
        }

        /// <summary>
        /// Retrieves a collection of services from the scope.
        /// </summary>
        /// <param name="serviceType">The collection of services to be retrieved.</param>
        /// <returns>The retrieved collection of services.</returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return DependencyResolverContainer.ResolveAll(serviceType);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        public void Dispose(bool disposing)
        {
            // Nothing to dispose
        }
    }
}