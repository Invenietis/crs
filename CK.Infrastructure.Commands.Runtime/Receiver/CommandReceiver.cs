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

        private FilterInfo[] _internalFilters = new FilterInfo []{
            new FilterInfo { Instance = new HandlerVerificationFilter(), Type = typeof(HandlerVerificationFilter) } };

        public async Task<ICommandResponse> ProcessCommandAsync( ICommandRequest commandRequest, IActivityMonitor monitor, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            var commandId = Guid.NewGuid();

            using( monitor.OpenTrace().Send( $"Processing new command [commandId={commandId}]..." ) )
            {
                var runtimeContext = CreateRuntimeContext( monitor, commandId, commandRequest, cancellationToken );
                var executionContext = new CommandExecutionContext( commandRequest.CommandDescription.Descriptor, runtimeContext );

                using( monitor.OpenTrace().Send( "Applying filters..." )
                    .ConcludeWith( () => executionContext.Response != null ? "INVALID" : "OK" ) )
                {
                    foreach( var filter in _internalFilters.Union( commandRequest.CommandDescription.Filters.Select( f => new FilterInfo { Type = f, Instance = _factory.CreateFilter( f ) } ) ) )
                    {
                        if( filter.Instance == null )
                        {
                            string msg = $"Unable to create the filter {filter.Type.FullName}.";
                            throw new InvalidOperationException( msg );
                        }

                        filter.Instance.OnCommandReceived( executionContext );
                    }
                }

                // Immediatly test if there is a response available.
                if( executionContext.Response != null ) return executionContext.Response;

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
                }

                return executionContext.Response; 
            }
        }

        internal CommandContext CreateRuntimeContext( IActivityMonitor monitor, Guid commandId, ICommandRequest request, CancellationToken cancellationToken )
        {
            Type commandContextType = typeof( CommandContext<> ).MakeGenericType( request.CommandDescription.Descriptor.CommandType );

            object instance = Activator.CreateInstance( commandContextType, monitor, request.Command, commandId, request.CommandDescription.Descriptor.IsLongRunning, request.CallbackId, cancellationToken   );
            return (CommandContext)instance;
        }

        class FilterInfo
        {
            public Type Type { get; set; }
            public ICommandFilter Instance { get; set; }
        }

        class HandlerVerificationFilter : ICommandFilter
        {
            public int Order
            {
                get { return -1000; }
            }

            public void OnCommandReceived( CommandExecutionContext executionContext )
            {
                if( executionContext.CommandDescription.HandlerType == null )
                {
                    string msg = $"No handler found for command [type={executionContext.CommandDescription.CommandType}].";
                    executionContext.RuntimeContext.Monitor.Warn().Send( msg );
                    executionContext.SetException( new InvalidOperationException( msg ) );
                }
            }
        }

    }
}
