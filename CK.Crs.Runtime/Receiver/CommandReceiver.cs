using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime
{
    public class CommandReceiver : ICommandReceiver
    {
        readonly IExecutionStrategySelector _executorSelector;
        readonly IFactories _factory;

        public CommandReceiver( IExecutionStrategySelector executor, IFactories factory )
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
                var filterContext = new FilterContext(monitor, commandRequest.CommandDescription, commandRequest.Command);

                using( monitor.OpenTrace().Send( "Applying filters..." )
                    .ConcludeWith( () => filterContext.IsRejected ? "INVALID" : "OK" ) )
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
                            await filter.Instance.OnCommandReceived( filterContext );

                            // Immediatly test if there is a response available.
                            if( filterContext.IsRejected )
                            {
                                return new CommandInvalidResponse( commandId, filterContext.RejectReason );
                            }
                        }
                    }
                }

                var executionContext = new CommandExecutionContext(
                    _factory.CreateExternalEventPublisher,
                    _factory.CreateCommandScheduler,
                    monitor,
                    commandRequest.Command,
                    commandId,
                    commandRequest.CommandDescription.Descriptor.IsLongRunning,
                    commandRequest.CallbackId,
                    cancellationToken );

                var context = new CommandContext( commandRequest.CommandDescription.Descriptor, executionContext );

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

            public Task OnCommandReceived( ICommandFilterContext context )
            {
                if( context.Description.Descriptor.HandlerType == null )
                {
                    string msg = $"No handler found for command [type={context.Description.Descriptor.CommandType}].";
                    context.Monitor.Warn().Send( msg );
                    context.Reject( msg );
                }

                return Task.FromResult<object>( null );
            }
        }
    }
}
