using System;
using CK.Core;
using CK.Crs;
using CK.Crs.Owin;
using CK.Crs.Runtime;
using CK.Crs.Runtime.Execution;
using CK.Crs.Runtime.Routing;
using Microsoft.Owin;

namespace Owin
{
    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CrsExtensions
    {
        public static IAppBuilder UseCrs( this IAppBuilder builder, string routePrefix, IServiceProvider applicationServices, Action<CrsConfiguration> configure = null )
        {
            if( routePrefix == null ) throw new ArgumentNullException( nameof( routePrefix ) );

            return builder.Map( routePrefix, ( app ) =>
            {
                var crsBuilder = new CrsBuilder( routePrefix, applicationServices);
                crsBuilder.ApplyDefaultConfigurationOrConfigure( configure );

                var commandReceiver = crsBuilder.Build( applicationServices );
                app.Use<CrsOwinMiddleware>( commandReceiver );
            } );
        }

        class CrsBuilder : CrsReceiverBuilder
        {
            IServiceProvider _services;
            public CrsBuilder( string routePrefix, IServiceProvider services ) : base( routePrefix, services.GetService<ICommandRegistry>() )
            {
                _services = services;
            }

            protected override void ConfigureDefaultPipeline( ICrsConfiguration config )
            {
                config.Pipeline
                    .Clear()
                    .UseMetaComponent()
                    .UseCommandRouter()
                    .UseJsonCommandBuilder()
                    .UseAmbientValuesValidator()
                    .UseFilters()
                    .UseTaskBasedCommandExecutor( Config.TraitContext )
                    .UseSyncCommandExecutor()
                    .UseJsonCommandWriter();

                config.ExternalComponents.CommandScheduler = new InMemoryScheduler( config, _services );
            }
        }
    }
}