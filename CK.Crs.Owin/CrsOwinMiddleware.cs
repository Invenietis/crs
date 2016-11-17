using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;
using CK.Crs.Runtime;
using System.IO;

namespace CK.Crs.Owin
{
    // You may need to install the Microsoft.AspNet.Http.Abstractions package into your project
    class CrsOwinMiddleware : OwinMiddleware
    {
        /// <summary>
        /// Shared options
        /// </summary>
        private readonly ICrsHandler _handler;

        public CrsOwinMiddleware( ICrsHandler handler ) : this( null, handler )
        {
        }

        public CrsOwinMiddleware( OwinMiddleware next, ICrsHandler handler )
            : base( next )
        {
            if( handler == null ) throw new ArgumentNullException( nameof( handler ) );

            _handler = handler;
        }

        public Task InvokeAsync( IDictionary<string, object> environment )
        {
            return Invoke( new OwinContext( environment ) );
        }

        public async override Task Invoke( IOwinContext context )
        {
            var connectionId = context.Request.Query["c"];
            var commandRequest = new CommandRequest( context.Request.Path.Value, context.Request.Body, context.Authentication.User, connectionId );
            var response = await _handler.ProcessCommandAsync( commandRequest, context.Response.Body );
            if( response != null )
            {
                string contentType;
                response.Headers.TryGetValue( "Content-Type", out contentType );
                context.Response.ContentType = contentType;

                foreach( var kv in response.Headers )
                    context.Response.Headers.Set( kv.Key, kv.Value );
            }
            if( response == null && Next != null )
                await Next.Invoke( context );
        }

    }
}
