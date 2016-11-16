using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CK.Crs.Runtime;
using Microsoft.AspNetCore.Http;
using CK.Crs;

namespace CK.Crs.AspNetCore
{
    // You may need to install the Microsoft.AspNet.Http.Abstractions package into your project
    class CrsMiddleware 
    {
        ICrsHandler _handler;
        RequestDelegate _next;

        /// <summary>
        /// Shared options
        /// </summary>
        public CrsMiddleware( RequestDelegate next, ICrsHandler handler )
        {
            if( handler == null ) throw new ArgumentNullException( nameof( handler ) );

            _next = next;
            _handler = handler;
        }

        public async Task Invoke( HttpContext context )
        {
            var connectionId = context.Request.Query["c"];
            var commandRequest = new CommandRequest( context.Request.Path.Value, context.Request.Body, context.User, connectionId );
            var response = await _handler.ProcessCommandAsync( commandRequest, context.Response.Body );
            if( response != null )
            {
                foreach( var kv in response.Headers )
                    context.Response.Headers.SetCommaSeparatedValues( kv.Key, kv.Value );
            }
            if( response == null && _next != null )
                await _next.Invoke( context );
        }

    }
}
