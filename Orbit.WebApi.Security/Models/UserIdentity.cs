using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbit.WebApi.Security.Models
{
    /// <summary>
    /// I User
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        long UserId { get; set; }

        /// <summary>
        /// Gets or sets the security identifier.
        /// </summary>
        /// <value>
        /// The security identifier.
        /// </value>
        Guid SecurityId { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        string Username { get; set; }

        /// <summary>
        /// Gets or sets the password timestamp.
        /// </summary>
        /// <value>
        /// The password timestamp.
        /// </value>
        long PasswordTimestamp { get; set; }
    }

    /// <summary>
    /// Role
    /// </summary>
    public class Role
    {
        /// <summary>
        /// Gets or sets the role identifier.
        /// </summary>
        /// <value>
        /// The role identifier.
        /// </value>
        public long RoleId { get; set; }

        /// <summary>
        /// Gets or sets the security identifier.
        /// </summary>
        /// <value>
        /// The security identifier.
        /// </value>
        public Guid SecurityId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
    }

    /// <summary>
    /// User authentication provider.
    /// </summary>
    public class AuthProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthProvider"/> class.
        /// </summary>
        public AuthProvider()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthProvider"/> class.
        /// </summary>
        /// <param name="loginProvider">The login provider.</param>
        /// <param name="providerKey">The provider key.</param>
        public AuthProvider(string loginProvider, string providerKey)
        {
            this.LoginProvider = loginProvider;
            this.ProviderKey = providerKey;
        }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public long UserId { get; set; }

        /// <summary>
        /// Gets or sets the login provider.
        /// </summary>
        /// <value>
        /// The login provider.
        /// </value>
        public string LoginProvider { get; set; }

        /// <summary>
        /// Gets or sets the provider key.
        /// </summary>
        /// <value>
        /// The provider key.
        /// </value>
        public string ProviderKey { get; set; }
    }

    /// <summary>
    /// User Identity
    /// </summary>
    /// <seealso cref="Extensions.Security.IUser" />
    public class UserIdentity : IUser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserIdentity"/> class.
        /// </summary>
        public UserIdentity()
        {
            this.Roles = new List<Role>();
            this.AuthProviders = new List<AuthProvider>(); 
        }

        /// <summary>
        /// Gets or sets the security identifier.
        /// </summary>
        /// <value>
        /// The security identifier.
        /// </value>
        public Guid SecurityId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public long UserId { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the roles.
        /// </summary>
        /// <value>
        /// The roles.
        /// </value>
        public ICollection<Role> Roles { get; set; }

        /// <summary>
        /// Gets or sets the authentication providers.
        /// </summary>
        /// <value>
        /// The authentication providers.
        /// </value>
        public ICollection<AuthProvider> AuthProviders { get; set; }

        /// <summary>
        /// Gets or sets the user authentication token identifier.
        /// </summary>
        /// <value>
        /// The user authentication token identifier.
        /// </value>
        public string UserAuthTokenId { get; set; }

        /// <summary>
        /// Gets or sets the password timestamp.
        /// </summary>
        /// <value>
        /// The password timestamp.
        /// </value>
        public long PasswordTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UserIdentity"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the user culture.
        /// </summary>
        /// <value>
        /// The user culture.
        /// </value>
        public string UserCulture { get; set; }
    }
}
