using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime.Pipeline
{
    public interface IPipeline : IDisposable
    {
        CommandRequest Request { get; }
        CommandAction Action { get; }
        CommandResponse Response { get; set; }
        /// <summary>
        /// Gets the <see cref="IActivityMonitor"/>
        /// </summary>
        IActivityMonitor Monitor { get; }

        PipelineEvents Events { get; }
    }

    class CommandReceivingPipeline : IPipeline, IDisposable
    {
        public PipelineEvents Events { get; }

        public CommandRequest Request { get; }

        public CommandAction Action { get; }
        /// <summary>
        /// Gets the <see cref="IActivityMonitor"/>
        /// </summary>
        public IActivityMonitor Monitor { get; }

        public CommandResponse Response { get; set; }

        IFactories _factories;
        IDisposableGroup _group;
        CancellationToken _cancellationToken;

        public CommandReceivingPipeline( PipelineEvents events, IFactories factories, IActivityMonitor monitor, CommandRequest request, CancellationToken cancellationToken )
        {
            Events = events;
            Monitor = monitor;
            Request = request;
            Action = new CommandAction( Guid.NewGuid(), Request );

            _factories = factories;
            _cancellationToken = cancellationToken;
            _group = Monitor.OpenTrace().Send( $"Processing new command [commandId={Action.CommandId}]..." );
        }

        public void Dispose()
        {
            _group.Dispose();
        }

        public Task<IPipeline> RouteCommand( ICommandRouteCollection routes )
        {
            return new CommandRouter( this, routes ).TryInvoke( _cancellationToken );
        }

        public Task<IPipeline> BuildCommand()
        {
            return new CommandBuilder( this ).TryInvoke( _cancellationToken );
        }

        public Task<IPipeline> ValidatedAmbientValues( IAmbientValues ambientValues )
        {
            return new AmbientValuesValidator( this, ambientValues ).TryInvoke( _cancellationToken );
        }

        public Task<IPipeline> InvokeFilters()
        {
            return new CommandFiltersInvoker( this, _factories ).TryInvoke( _cancellationToken );
        }

        public Task<IPipeline> ExecuteCommand( IExecutionStrategySelector selector )
        {
            return new CommandExecutor( this, _factories, selector ).TryInvoke( _cancellationToken );
        }
    }
}
