using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs.Runtime
{
    public class DefaultAmbientValueFactory : IAmbientValueProviderFactory
    {
        IServiceProvider _serviceProvider;
        public DefaultAmbientValueFactory( IServiceProvider serviceProvider )
        {
            _serviceProvider = serviceProvider;
        }

        public virtual IAmbientValueProvider Create( Type providerType ) => ActivatorUtilities.CreateInstance( _serviceProvider, providerType ) as IAmbientValueProvider;
    }
}
