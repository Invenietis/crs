using CK.Core;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs
{
    class DispatcherCommandContextNoWait<TCommand> : CommandContext
    {
        private readonly object _command;
        private readonly IServiceProvider _serviceProvider;

        public DispatcherCommandContextNoWait( Guid commandId, TCommand command, ICommandModel commandModel, CallerId callerId, IServiceProvider serviceProvider ) :
            base
            (
                commandId.ToString(),
                new ActivityMonitor(),
                commandModel,
                callerId
            )
        {
            _command = command;
            _serviceProvider = serviceProvider;
        }

        internal virtual Task<Response> Receive()
        {
            using( var serviceScope = _serviceProvider.CreateScope() )
            {
                var receiver = ServiceContainerExtension.GetService<ICommandReceiver>( serviceScope.ServiceProvider );
                try
                {
                    return receiver.ReceiveCommand( _command, this );
                }
                catch( Exception ex )
                {
                    Monitor.Error( ex );
                    throw;
                }
            }
        }
    }
}
