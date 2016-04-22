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
        public static IAppBuilder UseCrs( this IAppBuilder builder, string routePrefix, IServiceProvider applicationServices, Action<CrsConfiguration> configure = null )
        {
            if( routePrefix == null ) throw new ArgumentNullException( nameof( routePrefix ) );

            return builder.Map( routePrefix, ( app ) =>
            {
                var crsBuilder = new CrsBuilder( routePrefix, applicationServices);
                crsBuilder.ApplyDefaultConfigurationOrConfigure( configure );

                var commandReceiver = crsBuilder.Build( applicationServices );
                app.Use<CommandReceiverOwinMiddleware>( commandReceiver );
            } );
        }

        class CrsBuilder : CrsReceiverBuilder
        {
            public CrsBuilder( string routePrefix, IServiceProvider services ) : base( routePrefix, services ) { }

            protected override void ConfigureDefaultPipeline( IPipelineBuilder pipeline )
            {
                pipeline
                    .UseDefault()
                    .UseTaskBasedCommandExecutor( Config.TraitContext )
                    .UseSyncCommandExecutor()
                    .UseJsonCommandWriter();
            }
        }
    }
}