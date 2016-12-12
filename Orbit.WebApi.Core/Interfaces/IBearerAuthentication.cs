using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using Orbit.WebApi.Core.Common;

namespace Orbit.WebApi.Core.Interfaces
{
    /// <summary>
    /// interface IBearerAuthentication
    /// </summary>
    /// <seealso cref="Orbit.WebApi.Core.Interfaces.IAuthentication" />
    public interface IBearerAuthentication : IAuthentication
    {
        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        new ResponseError ErrorMessage { get; set; }
    }
}
