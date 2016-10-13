using System;
using CK.Core;
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
        public static IApplicationBuilder UseCrs( this IApplicationBuilder builder, string routePrefix, Action<CrsConfiguration> configure = null )
        {
            if( string.IsNullOrWhiteSpace( routePrefix ) ) throw new ArgumentNullException( nameof( routePrefix ) );

            return builder.Map( routePrefix, ( app ) =>
            {
                var crsBuilder = new CrsBuilder( routePrefix, app.ApplicationServices);
                crsBuilder.ApplyDefaultConfigurationOrConfigure( configure );

                var commandReceiver = crsBuilder.Build( builder.ApplicationServices  );
                app.UseMiddleware<CommandReceiverMiddleware>( commandReceiver );
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
