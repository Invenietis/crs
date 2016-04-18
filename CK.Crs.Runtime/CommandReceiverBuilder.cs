using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Core;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs.Runtime
{
    public class CommandReceiverBuilder
    {
        public ICommandReceiver InitializeCommandReceiver(
            ICommandRouteCollection routes,
            IServiceProvider applicationServices,
            IServiceScopeFactory scopeFactory,
            Action<CommandReceiverConfiguration> configure = null )
        {
            // Prepare routes configuration from command global registration
            var registry = applicationServices.GetRequiredService<ICommandRegistry>();
            var config = new CommandReceiverConfiguration( registry, routes );

            // Configures the command receiver
            if( configure != null ) configure( config );
            else ApplyDefaultConfiguration( config );

            // Creates the CommandReceiver
            return new CommandReceiver( applicationServices, scopeFactory, config );
        }


        protected virtual void ApplyDefaultConfiguration( CommandReceiverConfiguration config )
        {
            config.AddCommands( e => e.Registration );
        }
    }

}
