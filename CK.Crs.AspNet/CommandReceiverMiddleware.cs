using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CK.Crs.Pipeline;
using Microsoft.AspNetCore.Http;

namespace CK.Crs
{
    // You may need to install the Microsoft.AspNet.Http.Abstractions package into your project
    public class CommandReceiverMiddleware
    {
        private readonly ICommandReceiver _receiver;

        RequestDelegate _next;
        public CommandReceiverMiddleware( RequestDelegate next, ICommandReceiver receiver )
        {
            if( receiver == null ) throw new ArgumentNullException( nameof( receiver ) );

            _next = next;
            _receiver = receiver;
        }

        public async Task Invoke( HttpContext context )
        {
            var connectionId = context.Request.Query["c"];
            var request = new CommandRequest( context.Request.Path.Value,context.Request.Body, context.User, connectionId );
            await _receiver.ProcessCommandAsync( request, context.Response.Body, context.RequestAborted );
            if( _next != null )
                await _next.Invoke( context );
        }
    }
}
