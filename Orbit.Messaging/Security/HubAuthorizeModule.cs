using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Orbit.Messaging.Security
{
    /// <summary>
    /// Hub authorize module class.
    /// </summary>
    /// <seealso cref="HubPipelineModule" />
    public class HubAuthorizeModule : HubPipelineModule
    {
        /// <summary>
        /// This method is called after the connect components of any modules added later to the <see cref="T:Microsoft.AspNet.SignalR.Hubs.IHubPipeline" /> are
        /// executed and after <see cref="M:Microsoft.AspNet.SignalR.Hubs.IHub.OnConnected" /> is executed, if at all.
        /// </summary>
        /// <param name="hub">The hub the client has connected to.</param>
        protected override void OnAfterConnect(IHub hub)
        {
            base.OnAfterConnect(hub);
        }

        /// <summary>
        /// This method is called after the disconnect components of any modules added later to the <see cref="T:Microsoft.AspNet.SignalR.Hubs.IHubPipeline" /> are
        /// executed and after <see cref="M:Microsoft.AspNet.SignalR.Hubs.IHub.OnDisconnected(System.Boolean)" /> is executed, if at all.
        /// </summary>
        /// <param name="hub">The hub the client has disconnected from.</param>
        /// <param name="stopCalled">true, if stop was called on the client closing the connection gracefully;
        /// false, if the client timed out. Timeouts can be caused by clients reconnecting to another SignalR server in scaleout.</param>
        protected override void OnAfterDisconnect(IHub hub, bool stopCalled)
        {
            base.OnAfterDisconnect(hub, stopCalled);
        }

        /// <summary>
        /// This method is called after the incoming components of any modules added later to the <see cref="T:Microsoft.AspNet.SignalR.Hubs.IHubPipeline" />
        /// and the server-side hub method have completed execution.
        /// </summary>
        /// <param name="result">The return value of the server-side hub method</param>
        /// <param name="context">A description of the server-side hub method invocation.</param>
        /// <returns>
        /// The possibly new or updated return value of the server-side hub method
        /// </returns>
        protected override object OnAfterIncoming(object result, IHubIncomingInvokerContext context)
        {
            return base.OnAfterIncoming(result, context);
        }

        /// <summary>
        /// This method is called after the outgoing components of any modules added later to the <see cref="T:Microsoft.AspNet.SignalR.Hubs.IHubPipeline" /> are
        /// executed. This does not mean that all the clients have received the hub method invocation, but it does indicate indicate
        /// a hub invocation message has successfully been published to a message bus.
        /// </summary>
        /// <param name="context">A description of the client-side hub method invocation.</param>
        protected override void OnAfterOutgoing(IHubOutgoingInvokerContext context)
        {
            base.OnAfterOutgoing(context);
        }

        /// <summary>
        /// This method is called after the reconnect components of any modules added later to the <see cref="T:Microsoft.AspNet.SignalR.Hubs.IHubPipeline" /> are
        /// executed and after <see cref="M:Microsoft.AspNet.SignalR.Hubs.IHub.OnReconnected" /> is executed, if at all.
        /// </summary>
        /// <param name="hub">The hub the client has reconnected to.</param>
        protected override void OnAfterReconnect(IHub hub)
        {
            base.OnAfterReconnect(hub);
        }

        /// <summary>
        /// This method is called before the AuthorizeConnect components of any modules added later to the <see cref="T:Microsoft.AspNet.SignalR.Hubs.IHubPipeline" />
        /// are executed. If this returns false, then those later-added modules will not run and the client will not be allowed
        /// to subscribe to client-side invocations of methods belonging to the hub defined by the <see cref="T:Microsoft.AspNet.SignalR.Hubs.HubDescriptor" />.
        /// </summary>
        /// <param name="hubDescriptor">A description of the hub the client is trying to subscribe to.</param>
        /// <param name="request">The connect request of the client trying to subscribe to the hub.</param>
        /// <returns>
        /// true, if the client is authorized to connect to the hub, false otherwise.
        /// </returns>
        protected override bool OnBeforeAuthorizeConnect(HubDescriptor hubDescriptor, IRequest request)
        {
            return base.OnBeforeAuthorizeConnect(hubDescriptor, request);
        }

        /// <summary>
        /// This method is called before the connect components of any modules added later to the <see cref="T:Microsoft.AspNet.SignalR.Hubs.IHubPipeline" /> are
        /// executed. If this returns false, then those later-added modules and the <see cref="M:Microsoft.AspNet.SignalR.Hubs.IHub.OnConnected" /> method will
        /// not be run.
        /// </summary>
        /// <param name="hub">The hub the client has connected to.</param>
        /// <returns>
        /// true, if the connect components of later added modules and the <see cref="M:Microsoft.AspNet.SignalR.Hubs.IHub.OnConnected" /> method should be executed;
        /// false, otherwise.
        /// </returns>
        protected override bool OnBeforeConnect(IHub hub)
        {
            return base.OnBeforeConnect(hub);
        }

        /// <summary>
        /// This method is called before the disconnect components of any modules added later to the <see cref="T:Microsoft.AspNet.SignalR.Hubs.IHubPipeline" /> are
        /// executed. If this returns false, then those later-added modules and the <see cref="M:Microsoft.AspNet.SignalR.Hubs.IHub.OnDisconnected(System.Boolean)" /> method will
        /// not be run.
        /// </summary>
        /// <param name="hub">The hub the client has disconnected from.</param>
        /// <param name="stopCalled">true, if stop was called on the client closing the connection gracefully;
        /// false, if the client timed out. Timeouts can be caused by clients reconnecting to another SignalR server in scaleout.</param>
        /// <returns>
        /// true, if the disconnect components of later added modules and the <see cref="M:Microsoft.AspNet.SignalR.Hubs.IHub.OnDisconnected(System.Boolean)" /> method should be executed;
        /// false, otherwise.
        /// </returns>
        protected override bool OnBeforeDisconnect(IHub hub, bool stopCalled)
        {
            return base.OnBeforeDisconnect(hub, stopCalled);
        }

        /// <summary>
        /// This method is called before the incoming components of any modules added later to the <see cref="T:Microsoft.AspNet.SignalR.Hubs.IHubPipeline" /> are
        /// executed. If this returns false, then those later-added modules and the server-side hub method invocation will not
        /// be executed. Even if a client has not been authorized to connect to a hub, it will still be authorized to invoke
        /// server-side methods on that hub unless it is prevented in <see cref="M:Microsoft.AspNet.SignalR.Hubs.IHubPipelineModule.BuildIncoming(System.Func{Microsoft.AspNet.SignalR.Hubs.IHubIncomingInvokerContext,System.Threading.Tasks.Task{System.Object}})" /> by not
        /// executing the invoke parameter or prevented in <see cref="M:Microsoft.AspNet.SignalR.Hubs.HubPipelineModule.OnBeforeIncoming(Microsoft.AspNet.SignalR.Hubs.IHubIncomingInvokerContext)" /> by returning false.
        /// </summary>
        /// <param name="context">A description of the server-side hub method invocation.</param>
        /// <returns>
        /// true, if the incoming components of later added modules and the server-side hub method invocation should be executed;
        /// false, otherwise.
        /// </returns>
        protected override bool OnBeforeIncoming(IHubIncomingInvokerContext context)
        {
            return base.OnBeforeIncoming(context);
        }

        /// <summary>
        /// This method is called before the outgoing components of any modules added later to the <see cref="T:Microsoft.AspNet.SignalR.Hubs.IHubPipeline" /> are
        /// executed. If this returns false, then those later-added modules and the client-side hub method invocation(s) will not
        /// be executed.
        /// </summary>
        /// <param name="context">A description of the client-side hub method invocation.</param>
        /// <returns>
        /// true, if the outgoing components of later added modules and the client-side hub method invocation(s) should be executed;
        /// false, otherwise.
        /// </returns>
        protected override bool OnBeforeOutgoing(IHubOutgoingInvokerContext context)
        {
            return base.OnBeforeOutgoing(context);
        }

        /// <summary>
        /// This method is called before the reconnect components of any modules added later to the <see cref="T:Microsoft.AspNet.SignalR.Hubs.IHubPipeline" /> are
        /// executed. If this returns false, then those later-added modules and the <see cref="M:Microsoft.AspNet.SignalR.Hubs.IHub.OnReconnected" /> method will
        /// not be run.
        /// </summary>
        /// <param name="hub">The hub the client has reconnected to.</param>
        /// <returns>
        /// true, if the reconnect components of later added modules and the <see cref="M:Microsoft.AspNet.SignalR.Hubs.IHub.OnReconnected" /> method should be executed;
        /// false, otherwise.
        /// </returns>
        protected override bool OnBeforeReconnect(IHub hub)
        {
            return base.OnBeforeReconnect(hub);
        }

        /// <summary>
        /// This is called when an uncaught exception is thrown by a server-side hub method or the incoming component of a
        /// module added later to the <see cref="T:Microsoft.AspNet.SignalR.Hubs.IHubPipeline" />. Observing the exception using this method will not prevent
        /// it from bubbling up to other modules.
        /// </summary>
        /// <param name="exceptionContext">Represents the exception that was thrown during the server-side invocation.
        /// It is possible to change the error or set a result using this context.</param>
        /// <param name="invokerContext">A description of the server-side hub method invocation.</param>
        protected override void OnIncomingError(ExceptionContext exceptionContext, IHubIncomingInvokerContext invokerContext)
        {
            base.OnIncomingError(exceptionContext, invokerContext);
        }
    }
}