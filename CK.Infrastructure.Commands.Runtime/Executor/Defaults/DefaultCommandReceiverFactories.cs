using System;
using CK.Core;

namespace CK.Infrastructure.Commands
{
    public class DefaultCommandReceiverFactories : ICommandReceiverFactories
    {
        readonly DefaultCreateInstanceStrategy _s;

        public DefaultCommandReceiverFactories( IServiceProvider serviceProvider )
        {
            _s = new DefaultCreateInstanceStrategy( serviceProvider );
        }

        public ICommandDecorator CreateDecorator( Type decoratorType )
        {
            return _s.CreateInstanceOrDefault<ICommandDecorator>( decoratorType );
        }

        public ICommandFilter CreateFilter( Type filterType )
        {
            return _s.CreateInstanceOrDefault<ICommandFilter>( filterType );
        }

        public ICommandHandler CreateHandler( Type handlerType )
        {
            return _s.CreateInstanceOrDefault<ICommandHandler>( handlerType );
        }

        internal class DefaultCreateInstanceStrategy
        {
            readonly IActivityMonitor _monitor;
            readonly IServiceProvider _serviceProvider;
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
                    return inst ?? (defaultActivator != null ? defaultActivator() : Activator.CreateInstance( instanceType ) as T);
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
