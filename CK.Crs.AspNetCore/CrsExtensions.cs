using System;
using CK.Core;
using CK.Crs;
using CK.Crs.AspNetCore;
using CK.Crs.Runtime;
using CK.Crs.Runtime.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder
{
    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CrsExtensions
    {
        public static IApplicationBuilder UseCrsWithDefault(this IApplicationBuilder builder )
        {
            return UseCrs(builder, new PathString( "/commands" ) );
        }

        public static IApplicationBuilder UseCrs( this IApplicationBuilder builder, PathString routePrefix, Action<CrsConfiguration> configure = null )
        {
            if( routePrefix == null ) throw new ArgumentNullException( nameof( routePrefix ) );

            return builder.Map( routePrefix, ( app ) =>
            {
                var crsBuilder = new CrsBuilder( 
                    routePrefix, 
                    builder.ApplicationServices, 
                    builder.ApplicationServices.GetRequiredService<CommandReceiverOption>() );

                crsBuilder.ApplyDefaultConfigurationOrConfigure( configure );
                
                var commandReceiver = crsBuilder.Build(builder.ApplicationServices);

                app.UseMiddleware<CrsMiddleware>( commandReceiver );
            } );
        }

        class CrsBuilder : CrsReceiverBuilder
        {
            IServiceProvider _services;
            public CrsBuilder( string routePrefix, IServiceProvider services, CommandReceiverOption options ) 
                : base( routePrefix, options.Commands, options.Traits )
            {
                _services = services;
            }

            protected override void ConfigureDefault( CrsConfiguration config )
            {
                config.AddCommands(e => e.Registration);

                config.Pipeline
                    .Clear()
                    .UseMetaComponent(config.Routes)
                    .UseCommandRouter( config.Routes )
                    .UseJsonCommandBuilder()
                    .UseAmbientValuesValidator()
                    .UseFilters( config.Routes )
                    .UseTaskBasedCommandExecutor( Config.TraitContext )
                    .UseDefaultCommandExecutor()
                    .UseJsonCommandWriter();

                config.ExternalComponents.CommandScheduler = new InMemoryScheduler( config, _services );
            }
        }
    }
}