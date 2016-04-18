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
        public static IApplicationBuilder UseCommandReceiver( this IApplicationBuilder builder,
            PathString routePrefix,
            Action<CommandReceiverConfiguration> configure )
        {
            if( string.IsNullOrWhiteSpace( routePrefix ) ) throw new ArgumentNullException( nameof( routePrefix ) );

            return builder.Map( routePrefix, ( app ) =>
            {
                var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
                if( scopeFactory == null ) throw new InvalidOperationException( "An implementation of IServiceScopeFactory must exists in the DI Container" );

                var config = new CommandReceiverFinalBuilder();
                var commandReceiver = config.InitializeCommandReceiver(
                    new CommandRouteCollection( routePrefix.Value ),
                    app.ApplicationServices,
                    scopeFactory,
                    configure );

                app.UseMiddleware<CommandReceiverMiddleware>( commandReceiver );
            } );
        }
    }
}
