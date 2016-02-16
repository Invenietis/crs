using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{
    public class CommandReceiver : ICommandReceiver
    {
        readonly IExecutionStrategySelector _executorSelector;
        readonly ICommandReceiverFactories _factory;

        public CommandReceiver( IExecutionStrategySelector executor, ICommandReceiverFactories factory )
        {
            if( executor == null ) throw new ArgumentNullException( nameof( executor ) );
            if( factory == null ) throw new ArgumentNullException( nameof( factory ) );

            _executorSelector = executor;
            _factory = factory;
        }

        private FilterInfo[] _internalFilters = new FilterInfo []{
            new FilterInfo { Instance = new HandlerVerificationFilter(), Type = typeof(HandlerVerificationFilter) } };

        public async Task<CommandResponse> ProcessCommandAsync( CommandRequest commandRequest, IActivityMonitor monitor, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            var commandId = Guid.NewGuid();

            using( monitor.OpenTrace().Send( $"Processing new command [commandId={commandId}]..." ) )
            {
                var command = CreateCommand( monitor, commandId, commandRequest, cancellationToken );
                var context = new CommandContext( commandRequest.CommandDescription.Descriptor, command );

                using( monitor.OpenTrace().Send( "Applying filters..." )
                    .ConcludeWith( () => context.Result != null ? "INVALID" : "OK" ) )
                {
                    foreach( var filter in _internalFilters.Union( commandRequest.CommandDescription.Filters.Select( f => new FilterInfo { Type = f, Instance = _factory.CreateFilter( f ) } ) ) )
                    {
                        if( filter.Instance == null )
                        {
                            string msg = $"Unable to create the filter {filter.Type.FullName}.";
                            throw new InvalidOperationException( msg );
                        }
                        using( monitor.OpenTrace().Send( $"Executing filter {filter.Type.Name}" ) )
                        {
                            await filter.Instance.OnCommandReceived( context );

                            // Immediatly test if there is a response available.
                            if( context.Result != null )
                            {
                                return CommandResponse.CreateFromContext( context );
                            }
                        }
                    }
                }

                using( monitor.OpenTrace().Send( "Running command..." ) )
                {
                    var strategy = _executorSelector.SelectExecutionStrategy( context );
                    if( strategy == null )
                    {
                        string msg = "The Selector should returns a valid, non null executor... otherwise, commands will produce nothing!";
                        throw new ArgumentNullException( msg );
                    }

                    monitor.Trace().Send( $"[ExecutionStrategy={strategy.GetType().Name}]" );
                    return await strategy.ExecuteAsync( context );
                }
            }
        }

        internal Command CreateCommand( IActivityMonitor monitor, Guid commandId, CommandRequest request, CancellationToken cancellationToken )
        {
            Type commandContextType = typeof( Command<> ).MakeGenericType( request.CommandDescription.Descriptor.CommandType );

            object instance = Activator.CreateInstance( commandContextType, monitor, request.Command, commandId, request.CommandDescription.Descriptor.IsLongRunning, request.CallbackId, cancellationToken   );
            return (Command)instance;
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

            public Task OnCommandReceived( CommandContext context )
            {
                if( context.Description.HandlerType == null )
                {
                    string msg = $"No handler found for command [type={context.Description.CommandType}].";
                    context.Command.Monitor.Warn().Send( msg );
                    context.SetResult( new ValidationResult( msg ) );
                }

                return Task.FromResult<object>( null );
            }
        }
    }
}
