using System;
using System.Reflection;
using CK.Core;
using Microsoft.Extensions.DependencyInjection;
using Paramore.Brighter;

namespace CK.Crs.Runtime.Execution
{
    internal class DefaultExecutionFactory : IExecutionFactory, IAmAHandlerFactory, IAmAHandlerFactoryAsync
    {
        readonly DefaultCreateInstanceStrategy _s;

        public DefaultExecutionFactory( IServiceProvider serviceProvider )
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

        public IHandleRequestsAsync Create(Type handlerType)
        {
            return _s.CreateInstanceOrDefault<IHandleRequestsAsync>( handlerType );
        }

        IHandleRequests IAmAHandlerFactory.Create(Type handlerType)
        {
            return _s.CreateInstanceOrDefault<IHandleRequests>( handlerType );
        }

        public void Release(IHandleRequestsAsync handler)
        {
            if( handler is IDisposable)
            {
                ((IDisposable)handler).Dispose();
            }
        }

        public void Release(IHandleRequests handler)
        {
            if (handler is IDisposable)
            {
                ((IDisposable)handler).Dispose();
            }
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
