using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CK.Core;
using CK.Crs.Runtime;
using Microsoft.AspNetCore.Http;

namespace CK.Crs
{
    // You may need to install the Microsoft.AspNet.Http.Abstractions package into your project
    public class CommandReceiverMiddleware
    {
        /// <summary>
        /// Shared options
        /// </summary>
        private readonly ICommandRouteCollection _routes;
        private readonly ICommandReceiver _receiver;
        private readonly ICommandFormatter _formatter;

        RequestDelegate _next;
        public CommandReceiverMiddleware( RequestDelegate next, ICommandRouteCollection routes, ICommandReceiver receiver, ICommandFormatter formatter )
        {
            if( routes == null ) throw new ArgumentNullException( nameof( routes ) );
            if( receiver == null ) throw new ArgumentNullException( nameof( receiver ) );
            if( formatter == null ) throw new ArgumentNullException( nameof( formatter ) );

            _next = next;
            _routes = routes;
            _receiver = receiver;
            _formatter = formatter;
        }

        public async Task Invoke( HttpContext context )
        {
            // __types -> 
            var monitor = new Core.ActivityMonitor(context.Request.Path.Value );

            var routedCommandDescriptor = _routes.FindCommandDescriptor( context.Request.Path.Value );
            if( routedCommandDescriptor == null )
            {
                if( _next != null ) await _next.Invoke( context );
            }
            else
            {
                CommandRequest commandRequest =  _formatter.Deserialize( routedCommandDescriptor, context.Request.Body, null );
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
