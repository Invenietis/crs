using CK.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace CK.Crs
{
    public sealed class CrsMiddleware 
    {
        private readonly RequestDelegate _next;
        public CrsMiddleware( RequestDelegate next, IEndpointModel endpointModel, IResponseFormatter responseFormatter )
        {
            _next = next;
            EndpointModel = endpointModel;
            ResponseFormatter = responseFormatter;
        }

        public IEndpointModel EndpointModel { get; }
        public IResponseFormatter ResponseFormatter { get; }

        public async Task Invoke( HttpContext context )
        {
            ICrsEndpoint endpoint = new HttpCrsEndpoint( context );
            using( ICrsEndpointPipeline pipeline = endpoint.CreatePipeline( EndpointModel ) )
            {
                if( pipeline.IsValid )
                {
                    var response = await pipeline.ProcessCommand();
                    if( response != null )
                    {
                        await WriteResponse( context, response );
                        return;
                    }
                    pipeline.Monitor.Warn( "No response received from the command receiver..." );
                }
                else pipeline.Monitor.Warn( "Unable to receive the command" );
            }

            await _next( context );
        }

        private async Task WriteResponse( HttpContext context, Response response )
        {
            var result = ResponseFormatter.Format( response );
            if( result != null )
            {
                context.Response.Headers["ContentType"] = ResponseFormatter.ContentType;
                await context.Response.WriteAsync( result );
            }
        }
    }
}
