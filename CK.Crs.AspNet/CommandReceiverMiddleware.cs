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
            var commandRequest = new CommandRequest( context.Request.Path.Value,context.Request.Body, context.User );

            CommandResponse commandResponse = await _receiver.ProcessCommandAsync( commandRequest );
            if( commandResponse != null )
            {
                commandResponse.Write( context.Response.Body );
            }
            else
            {
                await _next?.Invoke( context );
            }
        }
    }
}
