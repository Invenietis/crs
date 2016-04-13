using System;
using CK.Crs.Runtime;
using CK.Crs.Runtime.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs
{

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CommandReceiverExtensions
    {
        public static IApplicationBuilder UseCommandReceiver( this IApplicationBuilder builder,
            PathString routePrefix,
            Action<CommandReceiverConfiguration> config )
        {
            if( string.IsNullOrWhiteSpace( routePrefix ) ) throw new ArgumentNullException( nameof( routePrefix ) );

            return builder.Map( routePrefix, ( app ) =>
            {
                var registry = builder.ApplicationServices.GetRequiredService<ICommandRegistry>();
                var events = builder.ApplicationServices.GetRequiredService<PipelineEvents>();

                var routeCollection = new CommandRouteCollection( routePrefix );
                var factory = new DefaultPipelineComponentFactory();
                var middlewareConfiguration = new CommandReceiverConfiguration( registry, routeCollection, factory );
                config( middlewareConfiguration );

                var commandReceiver = new CommandReceiver( app.ApplicationServices, middlewareConfiguration.Pipeline, events, routeCollection);
                app.UseMiddleware<CommandReceiverMiddleware>( commandReceiver );
            } );
        }

        public static void AddCommandReceiver( this IServiceCollection services, Action<CommandReceiverOption> configuration )
        {
            services.AddSingleton<IExecutionStrategySelector, BasicExecutionStrategySelector>();
            services.AddSingleton<IFactories, DefaultFactories>();
            services.AddSingleton<IAmbientValueProviderFactory, DefaultAmbientValueFactory>();
            services.AddSingleton<IAmbientValues, AmbientValues>();

            CommandReceiverOption options = new CommandReceiverOption( new CommandRegistry() );
            configuration( options );
            services.AddSingleton( options.Registry );
            services.AddSingleton( options.Events );
        }
    }
}
