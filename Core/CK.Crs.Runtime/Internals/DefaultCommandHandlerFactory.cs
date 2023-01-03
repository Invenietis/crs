using System;

namespace CK.Crs
{

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
                throw new InvalidOperationException( $"{typeof( T )} is not assignable from {instanceType}." );

            T inst = _activator.Create( instanceType ) as T;
            return inst;
        }

    }
}
