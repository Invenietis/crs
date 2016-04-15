using System;
using CK.Core;
using CK.Crs.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Owin;

namespace CK.Crs
{
    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CommandReceiverExtensions
    {
        public static IAppBuilder UseCommandReceiver( this IAppBuilder builder,
            IServiceProvider applicationServices,
            IServiceScopeFactory scopeFactory,
            PathString routePrefix,
            Action<CommandReceiverConfiguration> configure )
        {
            if( routePrefix == null ) throw new ArgumentNullException( nameof( routePrefix ) );

            return builder.Map( routePrefix, ( app ) =>
            {
                var commandReceiver = CrsConfig.InitializeCommandReceiver( routePrefix.Value, applicationServices, scopeFactory,  configure);
                app.Use<CommandReceiverOwinMiddleware>( commandReceiver );
            } );
        }
    }
}