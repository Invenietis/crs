using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.OptionsModel;

namespace CK.Infrastructure.Commands
{
    public class DefaultCommandReceiver : ICommandReceiver
    {
        readonly ICommandHandlerFactory _handlerFactory;
        readonly ICommandRunnerHostSelector _hostSelector;
        readonly IOptions<CommandReceiverOptions> _options;
        readonly ICommandValidator _validator;

        public DefaultCommandReceiver( ICommandValidator validator, ICommandRunnerHostSelector hostSelector, ICommandHandlerFactory handlerFactory, IOptions<CommandReceiverOptions> options )
        {
            if( validator == null ) throw new ArgumentNullException( nameof( validator ) );
            if( hostSelector == null ) throw new ArgumentNullException( nameof( hostSelector ) );
            if( handlerFactory == null ) throw new ArgumentNullException( nameof( handlerFactory ) );

            _validator = validator;
            _hostSelector = hostSelector;
            _handlerFactory = handlerFactory;
            _options = options;
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

            ICollection<ValidationResult> results;
            if( !_validator.TryValidate( executionContext, out results ) )
            {
                return executionContext.CreateInvalidResponse( results );
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

        internal CommandContext CreateRuntimeContext( Guid commandId, ICommandRequest request )
        {
            Type commandContextType = typeof( CommandContext<> ).MakeGenericType( request.CommandDescription.CommandType );

            bool isLongRunning = request.CommandDescription.IsLongRunning && _options.Value.EnableLongRunningSupport;
            object instance = Activator.CreateInstance( commandContextType, request.Command, commandId, isLongRunning, request.CallbackId  );
            return (CommandContext)instance;
        }
    }
}
