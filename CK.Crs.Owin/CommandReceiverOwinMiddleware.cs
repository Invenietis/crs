using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;
using CK.Crs.Pipeline;

namespace CK.Crs
{
    // You may need to install the Microsoft.AspNet.Http.Abstractions package into your project
    public class CommandReceiverOwinMiddleware : OwinMiddleware
    {
        /// <summary>
        /// Shared options
        /// </summary>
        private readonly ICommandReceiver _receiver;

        public CommandReceiverOwinMiddleware( ICommandReceiver receiver ) : this( null, receiver )
        {
        }

        public CommandReceiverOwinMiddleware( OwinMiddleware next, ICommandReceiver receiver )
            : base( next )
        {
            if( receiver == null ) throw new ArgumentNullException( nameof( receiver ) );

            _receiver = receiver;
        }

        public Task InvokeAsync( IDictionary<string, object> environment )
        {
            return Invoke( new OwinContext( environment ) );
        }

        public async override Task Invoke( IOwinContext context )
        {
            var connectionId = context.Request.Query["c"];
            var commandRequest = new CommandRequest( context.Request.Path.Value, context.Request.Body, context.Authentication.User, connectionId );
            await _receiver.ProcessCommandAsync( commandRequest, context.Response.Body );
            if ( Next != null )
                await Next.Invoke( context );
        }

    }
}
