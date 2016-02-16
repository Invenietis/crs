using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;

namespace CK.Crs
{


    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CommandReceiverExtensions
    {
        public static IApplicationBuilder UseCommandReceiver(
            this IApplicationBuilder builder,
            Microsoft.AspNet.Http.PathString routePrefix,
            Action<CommandReceiverConfiguration> config )
        {
            if( string.IsNullOrWhiteSpace( routePrefix ) ) throw new ArgumentNullException( nameof( routePrefix ) );

            return builder.Map( routePrefix, ( app ) =>
            {
                var registry = builder.ApplicationServices.GetRequiredService<ICommandRegistry>();
                var routeCollection = new CommandRouteCollection( routePrefix );
                var middlewareConfiguration = new CommandReceiverConfiguration( registry, routeCollection);
                config( middlewareConfiguration );
                app.UseOwin( pipeline =>
                {
                    var receiverMiddleware = new CommandReceiverMiddleware( null, routeCollection, builder.ApplicationServices );
                    pipeline( next => receiverMiddleware.InvokeAsync );
                } );
            } );
        }

        public static void AddCommandReceiver( this IServiceCollection services, Action<ICommandRegistry> configuration )
        {
            services.AddSingleton<ICommandReceiver, CommandReceiver>();
            services.AddSingleton<ICommandFormatter, DefaultCommandBinder>();
            services.AddSingleton<IExecutionStrategySelector, ExecutionStrategySelector>();
            services.AddSingleton<ICommandReceiverFactories, DefaultCommandReceiverFactories>();

            ICommandRegistry registry = new CommandRegistry();
            configuration( registry );
            services.AddInstance( registry );
        }
    }
}
