using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;

namespace CK.Crs
{
    class DefaultHandlerActivator : ICommandHandlerActivator
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ConcurrentDictionary<object, IServiceScope> _scopeEntries;
        public DefaultHandlerActivator( IServiceScopeFactory serviceScopeFactory )
        {
            _serviceScopeFactory = serviceScopeFactory;
            _scopeEntries = new ConcurrentDictionary<object, IServiceScope>();
        }

        public object Create( Type t )
        {
            var scope = _serviceScopeFactory.CreateScope();
            var handler = scope.ServiceProvider.GetService( t ) ?? ActivatorUtilities.CreateInstance( scope.ServiceProvider, t );
            if( _scopeEntries.ContainsKey( handler ) )
            {
                throw new InvalidOperationException( "A command handler MUST not be reuse and must be considered as transient" );
            }

            _scopeEntries[handler] = scope;
            return handler;
        }

        public void Release( object o )
        {
            if( o == null ) throw new ArgumentNullException( "o", "release must be called with a valid handler instance" );
            if( _scopeEntries.TryRemove( o, out var scope ) )
            {
                scope.Dispose();
            }
        }
    }
}
