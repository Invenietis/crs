using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;
using CK.Core;

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
        private readonly ICommandFormatter _formatter;

        public CommandReceiverMiddleware( OwinMiddleware next, ICommandRouteCollection routes, IServiceProvider serviceProvider ) : this(
            next,
            routes,
            serviceProvider.GetService<ICommandReceiver>(),
            serviceProvider.GetService<ICommandFormatter>() )
        {
        }

        public CommandReceiverMiddleware(
            OwinMiddleware next,
            ICommandRouteCollection routes,
            ICommandReceiver receiver,
            ICommandFormatter formatter
            ) : base( next )
        {
            if( routes == null ) throw new ArgumentNullException( nameof( routes ) );
            if( receiver == null ) throw new ArgumentNullException( nameof( receiver ) );
            if( formatter == null ) throw new ArgumentNullException( nameof( formatter ) );

            _routes = routes;
            _receiver = receiver;
            _formatter = formatter;
        }

        public Task InvokeAsync( IDictionary<string, object> environment )
        {
            return Invoke( new OwinContext( environment ) );
        }

        public async override Task Invoke( IOwinContext context )
        {
            // __types -> 
            var monitor = new Core.ActivityMonitor(context.Request.Path.Value );

            var routedCommandDescriptor = _routes.FindCommandDescriptor( context.Request.Path.Value );
            if( routedCommandDescriptor == null )
            {
                if( Next != null ) await Next.Invoke( context );
            }
            else
            {
                CommandRequest commandRequest =  _formatter.Deserialize( routedCommandDescriptor, context.Request.Body, context.Request.Environment );
                if( commandRequest == null )
                {
                    string msg = String.Format(
                        "A valid command definition has been infered from routes, but the command type {0} failed to be instanciated.",
                        routedCommandDescriptor.Descriptor.CommandType.Name );

                    throw new InvalidOperationException( msg );
                }

                CommandResponse commandResponse = await _receiver.ProcessCommandAsync( commandRequest, monitor );
                if( commandResponse == null )
                {
                    string msg = String.Format( "A valid command response must be received" );
                    throw new InvalidOperationException( msg );
                }

                _formatter.Serialize( commandResponse, context.Response.Body );
            }
        }

    }
}
