using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Core
{
    class DefaultAmbientValueFactory : IAmbientValueProviderFactory
    {
        IServiceProvider _serviceProvider;

        public DefaultAmbientValueFactory( IServiceProvider serviceProvider )
        {
            _serviceProvider = serviceProvider;
        }

        public IAmbientValueProvider Create( IAmbientValueProviderDescriptor descriptor ) =>  descriptor.Resolve( _serviceProvider );
    }
}
