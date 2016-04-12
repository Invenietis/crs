using System;
using CK.Core;
using CK.Crs.Runtime;
using CK.Crs.Runtime.Pipeline;
using Microsoft.Owin;
using Owin;

namespace CK.Crs
{

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CommandReceiverExtensions
    {
        public static IAppBuilder UseCommandReceiver( this IAppBuilder builder,
            ISimpleServiceContainer services,
            PathString routePrefix,
            Action<CommandReceiverConfiguration> config )
        {
            if( routePrefix == null ) throw new ArgumentNullException( nameof( routePrefix ) );

            return builder.Map( routePrefix, ( app ) =>
            {
                var registry = services.GetService( typeof( ICommandRegistry ) ) as ICommandRegistry;
                var events = services.GetService( typeof( PipelineEvents ) ) as PipelineEvents;
                var routeCollection = new CommandRouteCollection( routePrefix.Value );
                var middlewareConfiguration = new CommandReceiverConfiguration( registry, routeCollection, services );
                config( middlewareConfiguration );

                var commandReceiver = new CommandReceiver( services, middlewareConfiguration.Pipeline, events, routeCollection);
                app.Use<CommandReceiverOwinMiddleware>( commandReceiver );
            } );
        }

        public static void AddCommandReceiver( this ISimpleServiceContainer services, Action<CommandReceiverOption> configuration )
        {
            var factories = new DefaultFactories( services );
            services.Add( typeof( IFactories ), factories );
            services.Add( typeof( IExecutionStrategySelector ), new BasicExecutionStrategySelector( factories ) );
            services.Add( typeof( IAmbientValues ), new AmbientValues( new DefaultAmbientValueFactory( services ) ) );

            CommandReceiverOption options = new CommandReceiverOption( new CommandRegistry() );
            configuration( options );
            services.Add( typeof( ICommandRegistry ), options.Registry );
            services.Add( typeof( PipelineEvents ), options.Events );
        }
    }
}
