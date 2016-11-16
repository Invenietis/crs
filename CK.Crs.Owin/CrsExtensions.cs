using System;
using CK.Core;
using CK.Crs;
using CK.Crs.Owin;
using CK.Crs.Runtime;
using CK.Crs.Runtime.Execution;
using CK.Crs.Runtime.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;

namespace Owin
{
    public class CrsOptions
    {
        public CommandReceiverOption Receiver { get; set; }
        public CrsExecutorConfiguration Executor { get; set; }
    }
    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CrsExtensions
    {
        public static void AddCrs( this IServiceCollection services, Action<CrsOptions> options = null )
        {
            CrsOptions crsOptions = new Owin.CrsOptions();
            crsOptions.Receiver = services.AddCommandReceiver();

            options?.Invoke( crsOptions );

            services.AddCommandExecutor( o => o = crsOptions.Executor );
        }

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
            public CrsBuilder( string routePrefix, IServiceProvider services ) : base( routePrefix, services.GetRequiredService<ICommandRegistry>() )
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