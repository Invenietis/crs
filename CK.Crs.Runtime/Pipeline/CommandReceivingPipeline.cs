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

        IPipelineEvents Events { get; }
    }
    class CommandReceivingPipeline : IPipeline, IDisposable
    {
        public IPipelineEvents Events { get; }

        public CommandRequest Request { get; }

        public CommandAction Action { get; }
        /// <summary>
        /// Gets the <see cref="IActivityMonitor"/>
        /// </summary>
        public IActivityMonitor Monitor { get; }

        public CommandResponse Response { get; set; }

        IFactories _factories;
        ICommandRouteCollection _routes;
        IDisposableGroup _group;
        CancellationToken _cancellationToken;

        public CommandReceivingPipeline( IFactories factories, IActivityMonitor monitor, CommandRequest request, CancellationToken cancellationToken )
        {
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

        public Task UseCommandRouter( ICommandRouteCollection routes )
        {
            return new CommandRouter( this, routes ).Invoke( _cancellationToken );
        }

        public Task UseCommandBuilder()
        {
            return new CommandBuilder( this ).Invoke( _cancellationToken );
        }

        public Task UseAmbientValuesValidator( IAmbientValues ambientValues )
        {
            return new AmbientValuesValidator( this, ambientValues ).Invoke( _cancellationToken );
        }

        public Task UseFiltersInvoker()
        {
            return new CommandFiltersInvoker( this, _factories ).Invoke( _cancellationToken );
        }

        public Task UseCommandExecutor( IExecutionStrategySelector selector )
        {
            return new CommandExecutor( this, _factories, selector ).Invoke( _cancellationToken );
        }
    }
}
