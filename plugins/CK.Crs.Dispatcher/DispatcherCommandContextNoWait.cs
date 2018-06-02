using CK.Core;
using System;
using System.Threading.Tasks;

namespace CK.Crs
{
    class DispatcherCommandContextNoWait<TCommand> : CommandContext
    {
        private readonly object _command;
        private readonly IServiceProvider _serviceProvider;

        public DispatcherCommandContextNoWait( Guid commandId, TCommand command, ICommandModel commandModel, IServiceProvider serviceProvider ) :
            base
            (
                commandId.ToString(),
                new ActivityMonitor(),
                commandModel,
                CallerId.None
            )
        {
            _command = command;
            _serviceProvider = serviceProvider;
        }

        internal void ReceiveAndIgnoreResult()
        {
            try
            {
                var receiver = _serviceProvider.GetService<ICommandReceiver>();
                Task.Run( () => receiver.ReceiveCommand( _command, this ) );
            }
            catch( Exception ex )
            {
                Monitor.Error( ex );
                throw;
            }
        }
    }
}
