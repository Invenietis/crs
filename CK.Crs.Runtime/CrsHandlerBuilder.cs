using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Core;
using CK.Crs.Runtime;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs.Runtime
{
    /// <summary>
    /// Builds a <see cref="ICrsHandler"/>
    /// </summary>
    public class CrsHandlerBuilder<T> where T : ICrsConfiguration
    {
        T _config;

        protected T Config => _config;

        /// <summary>
        /// Adds the configuration object <see cref="ICrsConfiguration"/> to the builder.
        /// </summary>
        /// <param name="config">The <see cref="ICrsConfiguration"/> object</param>
        public void AddConfiguration( T config )
        {
            _config = config;
        }

        public void ApplyDefaultConfigurationOrConfigure( Action<T> configure )
        {
            if( configure != null ) configure( _config );
            else
            {
                ConfigureDefaultPipeline( _config );
                _config.AddCommands( e => e.Registration );
            }
        }

        /// <summary>
        /// Builds the <see cref="ICrsHandler"/> from the given <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="applicationServices">The application services</param>
        /// <param name="scopeFactory"></param>
        /// <returns></returns>
        public ICrsHandler Build( IServiceProvider applicationServices )
        {
            // Creates the CommandReceiver
            var scopeFactory = applicationServices.GetRequiredService<IServiceScopeFactory>();

            return new CrsPipelineHandler( scopeFactory, _config );
        }

        protected virtual void ConfigureDefaultPipeline( ICrsConfiguration configuration )
        {
        }
    }

}
