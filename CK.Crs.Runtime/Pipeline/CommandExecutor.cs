using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime.Pipeline
{
    class CommandExecutor : PipelineSlotBase
    {
        readonly IFactories _factories;
        readonly IExecutionStrategySelector _executorSelector;

        public CommandExecutor( CommandReceivingPipeline pipeline, IFactories factories )
            : this( pipeline, factories, new BasicExecutionStrategySelector( factories ) )
        {
        }

        public CommandExecutor( CommandReceivingPipeline pipeline, IFactories factories, IExecutionStrategySelector strategy ) : base( pipeline )
        {
            _factories = factories;
            _executorSelector = strategy;
        }

        public override Task Invoke( CancellationToken token )
        {
            if( Pipeline.Response == null )
            {
                CommandExecutionContext executionContext = CreateExecutionContext( token );

                var context = new CommandContext( Pipeline.Request.CommandDescription.Descriptor, executionContext );

                using( Pipeline.Request.Monitor.OpenTrace().Send( "Running command..." ) )
                {
                    var strategy = _executorSelector.SelectExecutionStrategy( context );
                    if( strategy == null )
                    {
                        string msg = "The Selector should returns a valid, non null executor... otherwise, commands will produce nothing!";
                        throw new ArgumentNullException( msg );
                    }

                    Pipeline.Request.Monitor.Trace().Send( $"[ExecutionStrategy={strategy.GetType().Name}]" );
                    return strategy.ExecuteAsync( context );
                }
            }

            return Task.FromResult( 0 );
        }

        private CommandExecutionContext CreateExecutionContext( CancellationToken token )
        {
            return new CommandExecutionContext(
                _factories.CreateExternalEventPublisher,
                _factories.CreateCommandScheduler,
                Pipeline.Request.Monitor,
                Pipeline.Request.Command,
                Pipeline.CommandId,
                Pipeline.Request.CommandDescription.Descriptor.IsLongRunning,
                Pipeline.Request.CallbackId,
                Pipeline.Request.User,
                token );
        }
    }
}
