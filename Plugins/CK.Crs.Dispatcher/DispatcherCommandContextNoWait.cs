using CK.Core;
using System;
using System.Threading.Tasks;

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
            var receiver = _serviceProvider.GetService<ICommandReceiver>();
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
