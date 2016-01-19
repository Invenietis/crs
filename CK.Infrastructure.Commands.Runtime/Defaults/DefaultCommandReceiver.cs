using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;
using Microsoft.Extensions.OptionsModel;

namespace CK.Infrastructure.Commands
{
    public class DefaultCommandReceiver : ICommandReceiver
    {
        readonly ICommandHandlerFactory _handlerFactory;
        readonly ICommandDecoratorFactory _decoratoryFactory;
        readonly ICommandRunnerHostSelector _hostSelector;
        readonly IOptions<CommandReceiverOptions> _options;
        readonly ICommandValidator _validator;

        public DefaultCommandReceiver( ICommandValidator validator, ICommandRunnerHostSelector hostSelector, ICommandHandlerFactory handlerFactory, ICommandDecoratorFactory decoratoryFactory, IOptions<CommandReceiverOptions> options )
        {
            if( validator == null ) throw new ArgumentNullException( nameof( validator ) );
            if( hostSelector == null ) throw new ArgumentNullException( nameof( hostSelector ) );
            if( handlerFactory == null ) throw new ArgumentNullException( nameof( handlerFactory ) );
            if( decoratoryFactory == null ) throw new ArgumentNullException( nameof( decoratoryFactory ) );

            _validator = validator;
            _hostSelector = hostSelector;
            _handlerFactory = handlerFactory;
            _decoratoryFactory = decoratoryFactory;
            _options = options;
        }


        public async Task<ICommandResponse> ProcessCommandAsync( ICommandRequest commandRequest, IActivityMonitor monitor, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            var commandId = Guid.NewGuid();

            using( monitor.OpenTrace().Send( $"Processing new command [commandId={commandId}]..." ) )
            {
                var runtimeContext = CreateRuntimeContext( monitor, commandId, commandRequest );
                var executionContext = new CommandExecutionContext( commandRequest.CommandDescription, runtimeContext );
                if( executionContext.CommandDescription.HandlerType == null )
                {
                    string msg = $"No handler found for command [type={commandRequest.CommandDescription.CommandType}].";
                    monitor.Warn().Send( msg );
                    return executionContext.CreateErrorResponse( msg );
                }

                ICollection<ValidationResult> results = null;

                using( monitor.OpenTrace().Send( "Validating command..." )
                    .ConcludeWith( () => results != null ? "INVALID" : "OK" ) )
                {
                    if( !_validator.TryValidate( executionContext, out results ) )
                    {
                        using( monitor.OpenWarn().Send( "Validation failed. See validation results below..." ) )
                        {
                            foreach( var validationResult in results )
                            {
                                string errorMessage = $"[{String.Join(", ", validationResult.MemberNames)}] --> {validationResult.ErrorMessage}";
                                monitor.Warn().Send( validationResult.ErrorMessage );
                            }
                        }
                        return executionContext.CreateInvalidResponse( results );
                    }
                }
                using( monitor.OpenTrace().Send( "Running command..." ) )
                {
                    ICommandRunnerHost host = _hostSelector.SelectHost( runtimeContext );
                    if( host == null )
                    {
                        string msg = "The ICommandRunnerHostSelector should returns a valid, non null, ICommandRunnerHost... otherwise, commands will produce nothing!";
                        throw new ArgumentNullException( msg );
                    }
                    monitor.Trace().Send( $"[hostType={host.GetType().Name}]" );
                    await host.HostJob( monitor, new CommandRunner( _handlerFactory, _decoratoryFactory ), executionContext, cancellationToken );

                    return executionContext.CreateResponse();
                }
            }
        }

        internal CommandContext CreateRuntimeContext( IActivityMonitor monitor, Guid commandId, ICommandRequest request )
        {
            Type commandContextType = typeof( CommandContext<> ).MakeGenericType( request.CommandDescription.CommandType );

            bool isLongRunning = request.CommandDescription.IsLongRunning && _options.Value.EnableLongRunningSupport;
            object instance = Activator.CreateInstance( commandContextType, monitor, request.Command, commandId, isLongRunning, request.CallbackId  );
            return (CommandContext)instance;
        }
    }
}
