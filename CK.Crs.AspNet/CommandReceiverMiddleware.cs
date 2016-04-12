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
            var response = await _receiver.ProcessCommandAsync( request );
            if( response != null )
            {
                response.Write( context.Response.Body );
            }
            else
            {
                await _next?.Invoke( context );
            }
        }
    }
}
