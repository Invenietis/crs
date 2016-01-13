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
        public static IApplicationBuilder UseCommandReceiver( this IApplicationBuilder builder, PathString routePrefix )
        {
            if( string.IsNullOrWhiteSpace( routePrefix ) ) throw new ArgumentNullException( nameof( routePrefix ) );

            return builder.Map( routePrefix, ( app ) =>
            {
                app.UseMiddleware<CommandReceiver>( routePrefix );
            } );
        }
        public static void AddCommandReceiver( this IServiceCollection services, bool enableLongRunningSupport = false )
            => AddCommandReceiver( services, ( o ) => o.EnableLongRunningSupport = enableLongRunningSupport );

        public static void AddCommandReceiver( this IServiceCollection services, Action<CommandReceiverOptions> options )
        {
            services.AddOptions();
            services.AddSingleton<ICommandReceiver, DefaultCommandReceiver>();
            services.AddSingleton<ICommandValidator, DefaultCommandValidator>();
            services.AddSingleton<ICommandBinder, DefaultCommandBinder>();
            services.AddSingleton<ICommandResponseSerializer, DefaultCommandResponseSerializer>();
            services.AddSingleton<ICommandRunnerHostSelector, CommandRunnerHostSelector>();
            services.AddSingleton<ICommandHandlerFactory, CommandHandlerFactory>();
            services.AddSingleton<ICommandFileWriter, DefaultCommandFileStore>();
            services.AddSingleton<ICommandResponseDispatcher>( ( sp ) =>
            {
                IConnectionManager connectionManager = sp.GetService<IConnectionManager>();
                if( connectionManager != null ) return new HubResponseDispatcher( connectionManager );

                return new NullResponseDispatcher();
            } );

            services.Configure( options );
        }
    }
}
