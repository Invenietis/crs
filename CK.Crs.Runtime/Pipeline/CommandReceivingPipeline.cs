using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime.Pipeline
{
    class CommandReceivingPipeline : IDisposable
    {
        public IPipelineEvents Events { get; }

        public Guid CommandId { get; }

        public CommandRequest Request { get; }

        public CommandResponse Response { get; set; }

        IFactories _factories;
        ICommandRouteCollection _routes;
        IDisposableGroup _group;
        CancellationToken _cancellationToken;

        public CommandReceivingPipeline( IFactories factories, CommandRequest request, CancellationToken cancellationToken )
        {
            _factories = factories;
            _cancellationToken = cancellationToken;

            CommandId = Guid.NewGuid();
            Request = request;
            Request.Monitor.OpenTrace().Send( $"Processing new command [commandId={CommandId}]..." );
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
