using System;
using System.Reflection;
using CK.Core;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs.Runtime.Execution
{
    public class DefaultFactories : ICommandExecutionFactories
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

        public virtual ICommandHandler CreateHandler( Type handlerType )
        {
            return _s.CreateInstanceOrDefault<ICommandHandler>( handlerType );
        }
        public virtual void ReleaseHandler( ICommandHandler handler )
        {
            var d = handler as IDisposable;
            if( d != null ) d.Dispose();
        }

        public virtual ICommandResponseDispatcher CreateResponseDispatcher()
        {
            return _s._serviceProvider.GetRequiredService<ICommandResponseDispatcher>();
        }

        public virtual IOperationExecutor<Event> CreateExternalEventPublisher()
        {
            return _s._serviceProvider.GetRequiredService<IOperationExecutor<Event>>();
        }

        public virtual IOperationExecutor<ScheduledCommand> CreateCommandScheduler()
        {
            return _s._serviceProvider.GetRequiredService<IOperationExecutor<ScheduledCommand>>();
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
