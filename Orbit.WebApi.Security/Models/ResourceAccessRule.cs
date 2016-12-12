using System;
using Orbit.WebApi.Base.SqlSerializer;

namespace Orbit.WebApi.Security.Models
{
    /// <summary>
    /// Class for Logical resource access rule.
    /// </summary>
    public class ResourceAccessRule
    {
        /// <summary>
        /// Gets or sets the name of the resource.
        /// </summary>
        /// <value>
        /// The name of the resource.
        /// </value>
        public string ResourceName { get; set; }

        /// <summary>
        /// Gets or sets the permission.
        /// </summary>
        /// <value>
        /// The permission.
        /// </value>
        [PropertyMapping("PermissionId")]
        public Int32 Permission { get; set; }

        /// <summary>
        /// Gets the permission string.
        /// </summary>
        /// <value>
        /// The permission string.
        /// </value>
        public string PermissionString
        {
            get
            {
                return ((Enums.Permission)this.Permission).ToString();
            }
        }
    }
}
