using System;
using CK.Crs.Runtime;
using CK.Crs.Runtime.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs
{

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CommandReceiverAspNetExtensions
    {
        public static IApplicationBuilder UseCommandReceiver( this IApplicationBuilder builder, PathString routePrefix, Action<CrsConfiguration> configure = null )
        {
            if( string.IsNullOrWhiteSpace( routePrefix ) ) throw new ArgumentNullException( nameof( routePrefix ) );

            return builder.Map( routePrefix, ( app ) =>
            {
                var crsBuilder = new CrsHandlerBuilder();

                var config = new CrsConfiguration( routePrefix.Value, new CommandRegistry(), new CommandRouteCollection() );
                if( configure != null ) configure( config );
                else ApplyDefaultConfiguration( config );

                crsBuilder.AddConfiguration( config );
                
                var scopeFactory = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
                var commandReceiver = crsBuilder.Build( builder.ApplicationServices, scopeFactory );

                app.UseMiddleware<CommandReceiverMiddleware>( commandReceiver );
            } );
        }

        private static void ApplyDefaultConfiguration( CrsConfiguration config )
        {
            config.Pipeline.UseDefault().UseSyncCommandExecutor().UseJsonResponseWriter();
        }
    }
}
