using System;
using System.Reflection;
using CK.Core;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs.Runtime
{
    public class DefaultFactories : IFactories
    {
        readonly DefaultCreateInstanceStrategy _s;

        public DefaultFactories( IServiceProvider serviceProvider )
        {
            _s = new DefaultCreateInstanceStrategy( serviceProvider );
        }

        public virtual ICommandDecorator CreateDecorator( Type decoratorType )
        {
            return _s.CreateInstanceOrDefault<ICommandDecorator>( decoratorType );
        }

        public virtual ICommandFilter CreateFilter( Type filterType )
        {
            return _s.CreateInstanceOrDefault<ICommandFilter>( filterType );
        }

        public virtual ICommandHandler CreateHandler( Type handlerType )
        {
            return _s.CreateInstanceOrDefault<ICommandHandler>( handlerType );
        }

        public virtual ICommandResponseDispatcher CreateResponseDispatcher()
        {
            return null;
        }
        public ICommandScheduler CreateCommandScheduler( ICommandExecutionContext context )
        {
#if NET451
            return new TransactionnalCommandScheduler( context );
#else
            return null;
#endif
        }

        public virtual IExternalEventPublisher CreateExternalEventPublisher( ICommandExecutionContext context )
        {
#if NET451
            return new TransactionnalEventPublisher( context );
#else
            return null;
#endif
        }

        public virtual void ReleaseHandler( ICommandHandler handler )
        {
            var d = handler as IDisposable;
            if( d != null ) d.Dispose();
        }

        public class DefaultCreateInstanceStrategy
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
