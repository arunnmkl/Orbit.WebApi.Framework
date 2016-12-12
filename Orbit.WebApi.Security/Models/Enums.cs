using System;

namespace Orbit.WebApi.Security.Models.Enums
{
    /// <summary>
    /// Permissions
    /// </summary>
    public enum Permission
    {
        /// <summary>
        /// The create
        /// </summary>
        Create = -1,

        /// <summary>
        /// The read
        /// </summary>
        Read = -2,

        /// <summary>
        /// The update
        /// </summary>
        Update = -3,

        /// <summary>
        /// The delete
        /// </summary>
        Delete = -4,

        /// <summary>
        /// The Allow
        /// </summary>
        Allow = -5,

        /// <summary>
        /// The Deny
        /// </summary>
        Deny = -6
    }

    /// <summary>
    /// Authorization Type
    /// </summary>
    [Flags]
    public enum AuthorizationType
    {
        /// <summary>
        /// The none
        /// </summary>
        None = 0,
        /// <summary>
        /// The access
        /// </summary>
        Access = 1,
        /// <summary>
        /// The audit failure
        /// </summary>
        AuditFailure = 2,
        /// <summary>
        /// The audit success
        /// </summary>
        AuditSuccess = 4,
    }
}
