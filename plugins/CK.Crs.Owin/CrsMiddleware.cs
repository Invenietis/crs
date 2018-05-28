using CK.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using System;
using System.Threading.Tasks;

namespace CK.Crs.Owin
{
    sealed class CrsOwinMiddleware : OwinMiddleware
    {
        private readonly IServiceScopeFactory _applicationServices;

        public CrsOwinMiddleware( OwinMiddleware next, IServiceScopeFactory applicationServices, IEndpointModel endpointModel )
            : base( next )
        {
            EndpointModel = endpointModel;
            _applicationServices = applicationServices;
        }

        public CrsOwinMiddleware( IServiceScopeFactory applicationServices, IEndpointModel endpointModel )
            : this( null, applicationServices, endpointModel )
        {
        }

        public IEndpointModel EndpointModel { get; }

        public async override Task Invoke( IOwinContext context )
        {
            using( var scope = _applicationServices.CreateScope() )
            {
                using( var endpoint = new HttpCrsEndpoint( context, scope.ServiceProvider ) )
                {
                    var pipeline = endpoint.CreatePipeline( new ActivityMonitor(), EndpointModel );

                    Response response = null;
                    if( pipeline.IsValid )
                    {
                        response = await pipeline.ProcessCommand();
                        if( response != null )
                        {
                            await WriteResponse( context, response );
                            return;
                        }
                        pipeline.Monitor.Warn( "No response received from the command receiver..." );
                    }
                    else pipeline.Monitor.Warn( "Unable to receive the command" );

                    if( response == null && Next != null )
                        await Next.Invoke( context );
                }
            }
        }

        private async Task WriteResponse( IOwinContext context, Response response )
        {
            var result = EndpointModel.ResponseFormatter.Format( response );
            if( result != null )
            {
                context.Response.Headers["Content-Type"] = EndpointModel.ResponseFormatter.ContentType;
                await context.Response.WriteAsync( result );
            }
        }
    }
}
