using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CK.Crs.Runtime;
using CK.Crs;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace CK.Crs.AspNetCore
{
    // You may need to install the Microsoft.AspNet.Http.Abstractions package into your project
    class CrsMiddleware
    {
        readonly ICrsHandler _handler;
        readonly RequestDelegate _next;

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
         
            var responseBuilder = new CommandResponseBuilder();
            await _handler.ProcessCommandAsync( commandRequest, responseBuilder );
                
            foreach( var kv in responseBuilder.Headers ) context.Response.Headers.SetCommaSeparatedValues( kv.Key, kv.Value );
            var hasBuiltResponse = await responseBuilder.WriteAsync(context.Response.Body);

            if( !hasBuiltResponse && _next != null ) await _next.Invoke( context );
        }
    }
}
