using CK.Core;
using CK.Crs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace CK.Crs.AspNetCore
{
    sealed class CrsMiddleware
    {
        private readonly RequestDelegate _next;
        public CrsMiddleware( RequestDelegate next, IEndpointModel endpointModel )
        {
            _next = next;
            EndpointModel = endpointModel;
        }

        public IEndpointModel EndpointModel { get; }

        public async Task Invoke( HttpContext context )
        {
            using( var endpoint = new HttpCrsEndpoint( context ) )
            {
                var pipeline = endpoint.CreatePipeline( new ActivityMonitor(), EndpointModel );
                if( pipeline.IsValid )
                {
                    var response = await pipeline.ProcessCommand().ConfigureAwait( false );
                    if( response != null )
                    {
                        await WriteResponse( context, response ).ConfigureAwait( false );
                        return;
                    }
                    pipeline.Monitor.Warn( "No response received from the command receiver..." );
                }
                else pipeline.Monitor.Warn( "Unable to receive the command" );
            }

            await _next( context ).ConfigureAwait( false );
        }

        private Task WriteResponse( HttpContext context, Response response )
        {
            var result = EndpointModel.ResponseFormatter.Format( response );
            if( result != null )
            {
                context.Response.Headers["Content-Type"] = EndpointModel.ResponseFormatter.ContentType;
                return context.Response.WriteAsync( result );
            }
            return Task.CompletedTask;
        }
    }
}
