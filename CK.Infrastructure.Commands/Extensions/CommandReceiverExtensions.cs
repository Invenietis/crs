using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Infrastructure.Commands
{


    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CommandReceiverExtensions
    {
        public static IApplicationBuilder UseCommandReceiver( 
            this IApplicationBuilder builder, 
            PathString routePrefix, 
            Action<CommandReceiverConfiguration> config  )
        {
            if( string.IsNullOrWhiteSpace( routePrefix ) ) throw new ArgumentNullException( nameof( routePrefix ) );

            return builder.Map( routePrefix, ( app ) =>
            {
                var registry = builder.ApplicationServices.GetRequiredService<ICommandRegistry>();
                var routeCollection = new CommandRouteCollection( routePrefix );
                var middlewareConfiguration = new CommandReceiverConfiguration( registry, routeCollection);
                config( middlewareConfiguration );
                app.UseMiddleware<CommandReceiverMiddleware>( routePrefix );
            } );
        }

        public static void AddCommandReceiver( this IServiceCollection services, Action<ICommandRegistry> configuration)
        {
            services.AddSingleton<ICommandReceiver, CommandReceiver>();
            services.AddSingleton<ICommandBinder, DefaultCommandBinder>();
            services.AddSingleton<ICommandResponseSerializer, DefaultCommandResponseSerializer>();
            services.AddSingleton<ICommandExecutorSelector, CommandExecutorSelector>();
            services.AddSingleton<ICommandReceiverFactories,DefaultCommandReceiverFactories>();
            services.AddSingleton<ICommandFileWriter, DefaultCommandFileStore>();
            services.AddSingleton<ICommandResponseDispatcher>( ( sp ) =>
            {
                IConnectionManager connectionManager = sp.GetService<IConnectionManager>();
                if( connectionManager != null ) return new HubResponseDispatcher( connectionManager );

                return new NullResponseDispatcher();
            } );

            ICommandRegistry registry = new CommandRegistry();
            configuration( registry );
            services.AddInstance( registry );
        }
    }
}
