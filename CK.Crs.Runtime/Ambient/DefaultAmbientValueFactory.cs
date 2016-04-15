using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Runtime
{
    public class DefaultAmbientValueFactory : IAmbientValueProviderFactory
    {
        IServiceProvider _serviceProvider;
        public DefaultAmbientValueFactory( IServiceProvider serviceProvider )
        {
            _serviceProvider = serviceProvider;
        }

        public virtual IAmbientValueProvider Create( Type providerType ) => _serviceProvider.GetService( providerType ) as IAmbientValueProvider;
    }
}
