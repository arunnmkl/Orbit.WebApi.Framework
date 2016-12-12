using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using Orbit.WebApi.Core.Dependency;
using Orbit.WebApi.Core.Interfaces;

namespace Orbit.WebApi.Core.Services
{
    /// <summary>
    /// This class is used to log the exception using ILog interface
    /// </summary>
    public class GlobalExceptionLogger : ExceptionLogger
    {
        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>The logger.</value>
        public ILog Logger { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalExceptionLogger" /> class.
        /// </summary>
        public GlobalExceptionLogger()
            : base()
        {
            Logger = DependencyResolverContainer.Resolve<ILog>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalExceptionLogger" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public GlobalExceptionLogger(ILog logger)
            : base()
        {
            Logger = logger ?? DependencyResolverContainer.Resolve<ILog>();
        }

        /// <summary>
        /// When overridden in a derived class, logs the exception asynchronously.
        /// </summary>
        /// <param name="context">The exception logger context.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous exception logging operation.</returns>
        public override Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            if (Logger != null)
            {
                Task.Run(() =>
                {
                    Logger.Log(context.Exception);
                });
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// When overridden in a derived class, logs the exception asynchronously.
        /// </summary>
        /// <param name="context">The exception logger context.</param>
        public override void Log(ExceptionLoggerContext context)
        {
            if (Logger != null)
            {
                Task.Run(() =>
                {
                    Logger.Log(context.Exception);
                });
            }
        }
    }
}