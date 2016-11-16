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
    /// <summary>
    /// Root Crs options used in Crs configuration
    /// </summary>
    public class CrsOptions
    {
        /// <summary>
        /// Receiver configuration
        /// </summary>
        public CommandReceiverOption Receiver { get; set; }
        /// <summary>
        /// Executor configuration
        /// </summary>
        public ExecutorOption Executor { get; set; }
        /// <summary>
        /// The JsonConverter to use
        /// </summary>
        public IJsonConverter JsonConverter { get; set; }
    }

    /// <summary>
    /// Extension method used to add the middleware to the HTTP request pipeline.
    /// </summary>
    public static class CrsExtensions
    {
        /// <summary>
        /// Add Crs services into the service collection.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        public static void AddCrs( this IServiceCollection services, Action<CrsOptions> options = null )
        {
            CrsOptions crsOptions = new Owin.CrsOptions();
            crsOptions.Receiver = services.AddCommandReceiver();

            options?.Invoke( crsOptions );
            if( crsOptions.JsonConverter == null ) crsOptions.UseJsonNet();
            services.AddSingleton( crsOptions.JsonConverter );

            services.AddCommandExecutor( o => o = crsOptions.Executor );
        }

        /// <summary>
        /// Registered Crs into the OWIN pipeline for the given routePrefix. 
        /// Multiple Crs can exists on different routes.
        /// </summary>
        /// <param name="builder"><see cref="IAppBuilder"/></param>
        /// <param name="routePrefix"></param>
        /// <param name="applicationServices"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Uses JSON.Net as the default Json Converter
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static CrsOptions UseJsonNet( this CrsOptions o )
        {
            o.JsonConverter = new JsonNetConverter();
            return o;
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