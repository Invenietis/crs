using System;
using CK.Core;
using CK.Crs.Runtime;
using CK.Crs.Runtime.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Owin;

namespace CK.Crs
{
    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CommandReceiverOwinExtensions
    {
        public static IAppBuilder UseCommandReceiver( this IAppBuilder builder,
            IServiceProvider applicationServices,
            IServiceScopeFactory scopeFactory,
            PathString routePrefix,
            Action<CommandReceiverConfiguration> configure )
        {
            if( routePrefix == null ) throw new ArgumentNullException( nameof( routePrefix ) );

            return builder.Map( routePrefix, ( app ) =>
            {
                var config = new CommandReceiverFinalBuilder();
                var commandReceiver = config.InitializeCommandReceiver(
                    new CommandRouteCollection( routePrefix.Value ),
                    applicationServices,
                    scopeFactory,
                    configure );

                app.Use<CommandReceiverOwinMiddleware>( commandReceiver );
            } );
        }
    }
}