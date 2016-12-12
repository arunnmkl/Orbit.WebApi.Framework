using System;

namespace Orbit.WebApi.Security.Models
{
    /// <summary>
    /// Class to encapsulate resource.
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// Gets or sets the resource identifier.
        /// </summary>
        /// <value>
        /// The resource identifier.
        /// </value>
        public Guid ResourceId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
    }
}
