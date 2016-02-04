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
    public class CommandReceiver : ICommandReceiver
    {
        readonly ICommandExecutorSelector _cmdExecutorSelector;
        readonly ICommandReceiverFactories _factory;

        public CommandReceiver( ICommandExecutorSelector cmdExecutorSelector, ICommandReceiverFactories factory )
        {
            if( cmdExecutorSelector == null ) throw new ArgumentNullException( nameof( cmdExecutorSelector ) );
            if( factory == null ) throw new ArgumentNullException( nameof( factory ) );

            _cmdExecutorSelector = cmdExecutorSelector;
            _factory = factory;
        }

        public async Task<ICommandResponse> ProcessCommandAsync( ICommandRequest commandRequest, IActivityMonitor monitor, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            var commandId = Guid.NewGuid();

            using( monitor.OpenTrace().Send( $"Processing new command [commandId={commandId}]..." ) )
            {
                var runtimeContext = CreateRuntimeContext( monitor, commandId, commandRequest );
                var executionContext = new CommandExecutionContext( commandRequest.CommandDescription.Descriptor, runtimeContext );
                if( executionContext.CommandDescription.HandlerType == null )
                {
                    string msg = $"No handler found for command [type={commandRequest.CommandDescription.Descriptor.CommandType}].";
                    monitor.Warn().Send( msg );
                    return executionContext.CreateErrorResponse( msg );
                }

                using( monitor.OpenTrace().Send( "Applying filters..." )
                    .ConcludeWith( () => executionContext.Response != null ? "INVALID" : "OK" ) )
                {
                    foreach( var filterType in commandRequest.CommandDescription.Filters )
                    {
                        var filter = _factory.CreateFilter( filterType );
                        if( filter == null )
                        {
                            string msg = $"Unable to create the filter {filterType.FullName}.";
                            throw new ArgumentNullException( msg );
                        }

                        filter.OnCommandReceived( executionContext );
                        if( executionContext.Response != null ) return executionContext.Response;
                    }
                }


                using( monitor.OpenTrace().Send( "Running command..." ) )
                {
                    var executor = _cmdExecutorSelector.SelectExecutor( runtimeContext );
                    if( executor == null )
                    {
                        string msg = "The Selector should returns a valid, non null executor... otherwise, commands will produce nothing!";
                        throw new ArgumentNullException( msg );
                    }
                    monitor.Trace().Send( $"[hostType={executor.GetType().Name}]" );
                    await executor.ExecuteAsync( executionContext, cancellationToken );

                    return executionContext.CreateResponse();
                }
            }
        }

        internal CommandContext CreateRuntimeContext( IActivityMonitor monitor, Guid commandId, ICommandRequest request )
        {
            Type commandContextType = typeof( CommandContext<> ).MakeGenericType( request.CommandDescription.Descriptor.CommandType );

            object instance = Activator.CreateInstance( commandContextType, monitor, request.Command, commandId, request.CommandDescription.Descriptor.IsLongRunning, request.CallbackId  );
            return (CommandContext)instance;
        }

        //internal void CreateInvalidResponse()
        //{
        //    using( monitor.OpenWarn().Send( "Validation failed. See validation results below..." ) )
        //    {
        //        foreach( var validationResult in results )
        //        {
        //            string errorMessage = $"[{String.Join(", ", validationResult.MemberNames)}] --> {validationResult.ErrorMessage}";
        //            monitor.Warn().Send( validationResult.ErrorMessage );
        //        }
        //    }
        //    return executionContext.CreateInvalidResponse( results );
        //}
    }
}
