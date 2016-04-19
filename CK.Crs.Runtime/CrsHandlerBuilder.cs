using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Core;
using CK.Crs.Runtime;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs.Runtime
{
    public class CrsHandlerBuilder
    {
        ICrsConfiguration _config;
        public void AddConfiguration( ICrsConfiguration config )
        {
            _config = config;
        }

        public ICrsHandler Build( IServiceProvider applicationServices, IServiceScopeFactory scopeFactory )
        {
            // Prepare routes configuration from command global registration
            //var registry = applicationServices.GetRequiredService<ICommandRegistry>();
            //var config = new CrsConfiguration( registry, routes );

            //ConfigureDefaultPipeline( config.Pipeline );
            //// Configures the command receiver
            //if( configure != null ) configure( config );
            //else ConfigureDefaultRegistration( config );

            // Creates the CommandReceiver
            return new CrsPipelineHandler( applicationServices, scopeFactory, _config );
        }

        protected virtual void ConfigureDefaultPipeline( IPipelineBuilder pipeline )
        {
        }
    }

}
