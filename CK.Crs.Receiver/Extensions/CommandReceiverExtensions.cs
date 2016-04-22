using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Crs.Runtime;
using CK.Crs.Runtime.Filtering;
using CK.Crs.Runtime.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;

namespace CK.Crs
{
    public static class CommandReceiverExtensions
    {
        public static void AddCommandReceiver( this IServiceCollection services, Action<CommandReceiverOption> registration )
        {
            services.AddCaching();
            services.AddSingleton<ICommandFilterFactory, DefaultCommandFilterFactory>();
            services.AddSingleton<IAmbientValueProviderFactory, DefaultAmbientValueFactory>();
            services.AddSingleton<IAmbientValues, AmbientValues>();

            var routes = new CommandRouteCollection();
            services.AddInstance<ICommandRouteCollection>( routes );
            services.AddInstance( routes );

            var r =  new CommandRegistry( services );
            var a = new AmbientValuesRegistration( services );
            var o = new CommandReceiverOption(r, a);

            registration( o );

            services.AddInstance<ICommandRegistry>( r );
            services.AddInstance<IAmbientValuesRegistration>( a );
        }
    }
}
