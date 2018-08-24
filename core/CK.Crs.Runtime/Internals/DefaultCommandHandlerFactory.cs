using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace CK.Crs
{
    public interface ICommandHandlerActivator
    {
        object Create( Type t );

        void Release( object o );
    }

    class DefaultHandlerActivator : ICommandHandlerActivator
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultHandlerActivator( IServiceProvider serviceProvider )
        {
            _serviceProvider = serviceProvider;
        }
        public object Create( Type t )
        {
            return _serviceProvider.GetService( t ) ?? ActivatorUtilities.CreateInstance( _serviceProvider, t );
        }

        public void Release( object o )
        {
        }
    }

    class DefaultCommandHandlerFactory : ICommandHandlerFactory
    {
        readonly ICommandHandlerActivator _activator;

        public DefaultCommandHandlerFactory( ICommandHandlerActivator activator )
        {
            _activator = activator;
        }

        ICommandHandler ICommandHandlerFactory.CreateHandler( Type handlerType )
        {
            var result = CreateInstanceOrDefault<ICommandHandler>( handlerType );
            return result;
        }

        void ICommandHandlerFactory.ReleaseHandler( ICommandHandler handler )
        {
            if( handler is IDisposable d ) d.Dispose();
            _activator.Release( handler );
        }

        T CreateInstanceOrDefault<T>( Type instanceType, Func<T> defaultActivator = null ) where T : class
        {
            if( !typeof( T ).IsAssignableFrom( instanceType ) )
                throw new InvalidOperationException( $"{typeof( T )} is not assignable from {instanceType}" );

            T inst = _activator.Create( instanceType ) as T;
            return inst;
        }

    }
}
