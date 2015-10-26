using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public class DefaultCommandReceiver : ICommandReceiver
    {
        readonly ICommandHandlerFactory _handlerFactory;
        readonly ICommandRunnerHostSelector _hostSelector;

        public DefaultCommandReceiver( ICommandRunnerHostSelector hostSelector, ICommandHandlerFactory handlerFactory )
        {
            if( hostSelector == null ) throw new ArgumentNullException( nameof( hostSelector ) );
            if( handlerFactory == null ) throw new ArgumentNullException( nameof( handlerFactory ) );

            _hostSelector = hostSelector;
            _handlerFactory = handlerFactory;
        }

        public async Task<ICommandResponse> ProcessCommandAsync( ICommandRequest commandRequest, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            var commandId = Guid.NewGuid();
            var runtimeContext = CreateRuntimeContext( commandId, commandRequest );
            var executionContext = new CommandExecutionContext( commandRequest.CommandDescription, runtimeContext );

            if( executionContext.CommandDescription.HandlerType == null )
            {
                string msg = "Handler not found for command type " + commandRequest.CommandDescription.CommandType;
                return executionContext.CreateErrorResponse( msg );
            }

            ICommandRunnerHost host = _hostSelector.SelectHost( runtimeContext );
            if( host == null )
            {
                string msg = "The ICommandRunnerHostSelector should returns a valid, non null, ICommandRunnerHost... otherwise, commands will produce nothing!";
                throw new ArgumentNullException( msg );
            }

            await host.HostJob( new CommandRunner( _handlerFactory ), executionContext, cancellationToken );

            return executionContext.CreateResponse();
        }

        static internal CommandContext CreateRuntimeContext( Guid commandId, ICommandRequest request )
        {
            Type commandContextType = typeof( CommandContext<> ).MakeGenericType( request.CommandDescription.CommandType );
            object instance = Activator.CreateInstance( commandContextType, request.Command, commandId, request.CommandDescription.IsLongRunning, request.CallbackId  );
            return (CommandContext)instance;
        }
    }
}
