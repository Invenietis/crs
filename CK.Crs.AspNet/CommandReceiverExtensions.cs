using System;
using CK.Crs.Runtime;
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
                var routeCollection = new CommandRouteCollection( routePrefix );
                var middlewareConfiguration = new CommandReceiverConfiguration( registry, routeCollection);
                config( middlewareConfiguration );
                app.UseMiddleware<CommandReceiverMiddleware>( routeCollection );
            } );
        }

        public static void AddCommandReceiver( this IServiceCollection services, Action<ICommandRegistry> configuration )
        {
            services.AddSingleton<ICommandReceiver, CommandReceiver>();
            services.AddSingleton<ICommandFormatter, DefaultCommandFormatter>();
            services.AddSingleton<IExecutionStrategySelector, BasicExecutionStrategySelector>();
            services.AddSingleton<IFactories, DefaultFactories>();

            ICommandRegistry registry = new CommandRegistry();
            configuration( registry );
            services.AddSingleton( registry );
        }
    }
}
