using Microsoft.Extensions.DependencyInjection;
using System;

namespace CK.Crs
{
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
}
