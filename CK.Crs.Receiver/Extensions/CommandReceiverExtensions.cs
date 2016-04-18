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
        public static void AddCommandReceiver( this IServiceCollection services, Action<CommandReceiverOption> configuration )
        {
            services.AddSingleton<IAmbientValueProviderFactory, DefaultAmbientValueFactory>();
            services.AddSingleton<IAmbientValues, AmbientValues>();

            CommandReceiverOption options = new CommandReceiverOption( new CommandRegistry() );
            configuration( options );
            services.AddInstance( options.Registry );
        }
    }
}
