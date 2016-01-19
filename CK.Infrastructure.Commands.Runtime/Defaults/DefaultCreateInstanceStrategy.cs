using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    class DefaultCreateInstanceStrategy
    {
        readonly IServiceProvider _serviceProvider;
        public DefaultCreateInstanceStrategy( IServiceProvider serviceProvider )
        {
            _serviceProvider = serviceProvider;
        }

        public T CreateInstanceOrDefault<T>( Type instanceType, Func<T> defaultActivator = null ) where T : class
        {
            if( !typeof( T ).IsAssignableFrom( instanceType ) )
                throw new InvalidOperationException( $"{typeof( T )} is not assignable from {instanceType}" );

            T inst = _serviceProvider.GetService( instanceType ) as T;
            return inst ?? (defaultActivator != null ? defaultActivator() : Activator.CreateInstance<T>());
        }
    }
}
