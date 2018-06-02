using System;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{
    public class CrsCommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _service;

        public CrsCommandDispatcher( IServiceProvider service )
        {
            _service = service;
        }

        public void Send<TCommand>( Guid commandId, TCommand command )
        {
            var commandModel = _service.GetService<ICommandRegistry>().GetCommandByName( new CommandName( typeof( TCommand ) ) );
            if( commandModel == null ) throw new ArgumentException( "Command not found" );

            var context = new DispatcherCommandContextNoWait<TCommand>( commandId, command, commandModel, _service ); ;
            context.ReceiveAndIgnoreResult();
        }

        public Task<TResult> SendAndWaitResult<TCommand, TResult>( Guid commandId, TCommand command ) where TCommand : ICommand<TResult>
        {
            var commandModel = _service.GetService<ICommandRegistry>().GetCommandByName( new CommandName( typeof( TCommand ) ) );
            if( commandModel == null ) throw new ArgumentException( "Command not found" );

            var context = new DispatcherCommandContext<TCommand, TResult>( commandId, command, commandModel, _service ); ;
            return context.Receive();
        }
    }
}
