using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace CK.Crs
{
    public class DefaultRequestHandlerFactory : IRequestHandlerFactory
    {
        readonly IServiceProvider _services;

        public DefaultRequestHandlerFactory( IServiceProvider services )
        {
            _services = services;
        }

        IRequestHandler IRequestHandlerFactory.CreateHandler( Type handlerType )
        {
            var result = CreateInstanceOrDefault<IRequestHandler>( handlerType );
            return result;
        }

        void IRequestHandlerFactory.ReleaseHandler( IRequestHandler handler )
        {
            if( handler is IDisposable d ) d.Dispose();
        }

        T CreateInstanceOrDefault<T>( Type instanceType, Func<T> defaultActivator = null ) where T : class
        {
            if( !typeof( T ).IsAssignableFrom( instanceType ) )
                throw new InvalidOperationException( $"{typeof( T )} is not assignable from {instanceType}" );

            T inst = _services.GetService( instanceType ) as T;
            return inst ?? (defaultActivator != null ? defaultActivator() : ActivatorUtilities.CreateInstance( _services, instanceType ) as T);
        }

    }
}
