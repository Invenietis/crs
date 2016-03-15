using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;
using CK.Crs.Runtime;

namespace CK.Crs
{
    // You may need to install the Microsoft.AspNet.Http.Abstractions package into your project
    public class CommandReceiverMiddleware : OwinMiddleware
    {
        /// <summary>
        /// Shared options
        /// </summary>
        private readonly ICommandRouteCollection _routes;
        private readonly ICommandReceiver _receiver;

        public CommandReceiverMiddleware( OwinMiddleware next, ICommandRouteCollection routes, ICommandReceiver receiver )
            : base( next )
        {
            if( routes == null ) throw new ArgumentNullException( nameof( routes ) );
            if( receiver == null ) throw new ArgumentNullException( nameof( receiver ) );

            _routes = routes;
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
            var commandResponse = await _receiver.ProcessCommandAsync(_routes, commandRequest );
            if( commandResponse != null )
            {
                commandResponse.Write( context.Response.Body );
            }
            else
            {
                await Next?.Invoke( context );
            }
        }

    }
}
