using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Crs.Runtime.Filtering;
using CK.Crs.Runtime.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs
{
    public static class CommandReceiverExtensions
    {
        public static void AddCommandReceiver( this IServiceCollection services, Action<ICommandRegistry> registration )
        {
            services.AddSingleton<IAmbientValueProviderFactory, DefaultAmbientValueFactory>();
            services.AddSingleton<IAmbientValues, AmbientValues>();

            var routes = new CommandRouteCollection();
            services.AddInstance<ICommandRouteCollection>( routes );
            services.AddInstance( routes );

            var r =  new CommandRegistry();
            registration( r );
            services.AddInstance<ICommandRegistry>( r );
        }
    }
}
