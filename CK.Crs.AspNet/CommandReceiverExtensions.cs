using System;
using CK.Crs.Runtime;
using Microsoft.AspNet.Builder;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddSingleton<ICommandFormatter, DefaultCommandFormatter>();
            services.AddSingleton<IExecutionStrategySelector, BasicExecutionStrategySelector>();
            services.AddSingleton<IFactories, DefaultFactories>();
            services.AddSingleton<IPrincipalAccessor, AspNetPrincipalAccessor>();

            ICommandRegistry registry = new CommandRegistry();
            configuration( registry );
            services.AddInstance( registry );
        }
    }
}
