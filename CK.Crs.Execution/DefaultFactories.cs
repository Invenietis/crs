using System;
using System.Reflection;
using CK.Core;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs.Runtime.Execution
{
    public class DefaultFactory : ICommandHandlerFactory
    {
        readonly DefaultCreateInstanceStrategy _s;

        public DefaultFactory( IServiceProvider serviceProvider )
        {
            _s = new DefaultCreateInstanceStrategy( serviceProvider );
        }

        public virtual ICommandDecorator CreateDecorator( Type decoratorType )
        {
            return _s.CreateInstanceOrDefault<ICommandDecorator>( decoratorType );
        }

        public virtual ICommandHandler CreateHandler( Type handlerType )
        {
            return _s.CreateInstanceOrDefault<ICommandHandler>( handlerType );
        }
        public virtual void ReleaseHandler( ICommandHandler handler )
        {
            var d = handler as IDisposable;
            if( d != null ) d.Dispose();
        }

        public class DefaultCreateInstanceStrategy
        {
            readonly IActivityMonitor _monitor;
            internal readonly IServiceProvider _serviceProvider;
            public DefaultCreateInstanceStrategy( IServiceProvider serviceProvider )
            {
                _serviceProvider = serviceProvider;
                _monitor = new ActivityMonitor( "DefaultCreateInstanceStrategy" );
            }

            public T CreateInstanceOrDefault<T>( Type instanceType, Func<T> defaultActivator = null ) where T : class
            {
                if( !typeof( T ).IsAssignableFrom( instanceType ) )
                    throw new InvalidOperationException( $"{typeof( T )} is not assignable from {instanceType}" );
                try
                {
                    T inst = _serviceProvider.GetService( instanceType ) as T;
                    return inst ?? (defaultActivator != null ? defaultActivator() : ActivatorUtilities.CreateInstance( _serviceProvider, instanceType ) as T);
                }
                catch( Exception ex )
                {
                    _monitor.Error().Send( ex );
                    return null;
                }
            }
        }
    }

}
