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
        private readonly ICommandRouteCollection _routes;

        RequestDelegate _next;
        public CommandReceiverMiddleware( RequestDelegate next, ICommandRouteCollection routes, ICommandReceiver receiver )
        {
            if( routes == null ) throw new ArgumentNullException( nameof( routes ) );
            if( receiver == null ) throw new ArgumentNullException( nameof( receiver ) );

            _next = next;
            _routes = routes;
            _receiver = receiver;
        }

        public async Task Invoke( HttpContext context )
        {
            var request = new CommandRequest( context.Request.Path.Value,context.Request.Body, context.User );
            var response = await _receiver.ProcessCommandAsync( _routes, request );
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
