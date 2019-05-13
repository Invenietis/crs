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

        public Task<Response> Send( Guid commandId, object command, Type commandType, CallerId callerId )
        {
            var commandModel = _service.GetService<ICommandRegistry>().GetCommandByName( new CommandName( commandType ) );
            if( commandModel == null ) throw new ArgumentException( "Command not found" );

            var context = new DispatcherCommandContextNoWait<object>( commandId, command, commandModel, callerId, _service ); ;
            return context.Receive();
        }

        public Task<Response> Send<TCommand>( Guid commandId, TCommand command, CallerId callerId )
        {
            var commandModel = _service.GetService<ICommandRegistry>().GetCommandByName( new CommandName( typeof( TCommand ) ) );
            if( commandModel == null ) throw new ArgumentException( "Command not found" );

            var context = new DispatcherCommandContextNoWait<TCommand>( commandId, command, commandModel, callerId, _service ); ;
            return context.Receive();
        }

        public Task<TResult> SendAndWaitResult<TCommand, TResult>( Guid commandId, TCommand command, CallerId callerId ) where TCommand : ICommand<TResult>
        {
            var commandModel = _service.GetService<ICommandRegistry>().GetCommandByName( new CommandName( typeof( TCommand ) ) );
            if( commandModel == null ) throw new ArgumentException( "Command not found" );

            var context = new DispatcherCommandContext<TCommand, TResult>( commandId, command, commandModel, callerId, _service ); ;
            return context.Receive();
        }
    }
}
