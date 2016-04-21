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
        public static IAppBuilder UseCommandReceiver( this IAppBuilder builder, PathString routePrefix, IServiceProvider applicationServices, IServiceScopeFactory scopeFactory,
            Action<CrsConfiguration> configure = null )
        {
            if( routePrefix == null ) throw new ArgumentNullException( nameof( routePrefix ) );

            return builder.Map( routePrefix, ( app ) =>
            {
                var crsBuilder = new CrsHandlerBuilder();

                var registry = applicationServices.GetRequiredService<ICommandRegistry>();
                var routeCollection = applicationServices.GetRequiredService<CommandRouteCollection>();
                var config = new CrsConfiguration( routePrefix.Value, registry, routeCollection );
                if( configure != null ) configure( config );
                else ApplyDefaultConfiguration( config );

                crsBuilder.AddConfiguration( config );

                var commandReceiver = crsBuilder.Build( applicationServices, scopeFactory );

                app.Use<CommandReceiverOwinMiddleware>( commandReceiver );
            } );
        }

        private static void ApplyDefaultConfiguration( CrsConfiguration config )
        {
            config.Pipeline.UseDefault().UseSyncCommandExecutor().UseJsonCommandWriter();
        }
    }
}