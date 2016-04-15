using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Core;
using CK.Crs.Runtime.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs.Runtime
{
    public static class CrsConfig
    {
        public static ICommandReceiver InitializeCommandReceiver( string routePrefix, IServiceProvider applicationServices, IServiceScopeFactory scopeFactory, Action<CommandReceiverConfiguration> configure = null )
        {
            // Prepare routes configuration from command global registration
            var registry = applicationServices.GetService( typeof( ICommandRegistry ) ) as ICommandRegistry;
            var config = new CommandReceiverConfiguration( routePrefix, registry );

            // Configures the command receiver
            if( configure != null ) configure( config );
            else ApplyDefaultConfiguration( config );
            
            // Creates the CommandReceiver
            return new CommandReceiver( applicationServices, scopeFactory, config );
        }


        private static void ApplyDefaultConfiguration( CommandReceiverConfiguration config )
        {
            config.AddCommands( e => e.Registration );
        }

        public static void AddCommandReceiver( this IServiceCollection services, Action<CommandReceiverOption> configuration )
        {
            services.AddSingleton<IExecutionStrategySelector, BasicExecutionStrategySelector>();
            services.AddSingleton<IFactories, DefaultFactories>();
            services.AddSingleton<IAmbientValueProviderFactory, DefaultAmbientValueFactory>();
            services.AddSingleton<IAmbientValues, AmbientValues>();

            CommandReceiverOption options = new CommandReceiverOption( new CommandRegistry() );
            configuration( options );
            services.AddInstance( options.Registry );
        }
    }

}
