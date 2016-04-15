using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime.Pipeline
{
    class CommandExecutor : PipelineComponent
    {
        readonly IFactories _factories;
        readonly IExecutionStrategySelector _executorSelector;

        public CommandExecutor( IFactories factories )
            : this( factories, new BasicExecutionStrategySelector( factories ) )
        {
        }

        public CommandExecutor( IFactories factories, IExecutionStrategySelector strategy )
        {
            _factories = factories;
            _executorSelector = strategy;
        }

        public override bool ShouldInvoke( IPipeline pipeline )
        {
            return pipeline.Response == null && pipeline.Action.Command != null;
        }

        public override async Task Invoke( IPipeline pipeline, CancellationToken token )
        {
            CommandExecutionContext executionContext = new CommandExecutionContext(
                    pipeline.Action,
                    pipeline.Monitor,
                    token,
                    _factories.CreateExternalEventPublisher,
                    _factories.CreateCommandScheduler);

            var context = new CommandContext( executionContext );

            using( pipeline.Monitor.OpenTrace().Send( "Running command..." ) )
            {
                var strategy = _executorSelector.SelectExecutionStrategy( context );
                if( strategy == null )
                {
                    string msg = "The Selector should returns a valid, non null executor... otherwise, commands will produce nothing!";
                    throw new ArgumentNullException( msg );
                }

                pipeline.Monitor.Trace().Send( $"[ExecutionStrategy={strategy.GetType().Name}]" );

                if( pipeline.Configuration.Events.CommandExecuting != null )
                    await pipeline.Configuration.Events.CommandExecuting?.Invoke( context );

                pipeline.Response = await strategy.ExecuteAsync( context );

                // REVIEW: the context should be different 
                // because at this stage we do not want to offe the possibility to sets the result
                //await Pipeline.Events.CommandExecuted?.Invoke( context );
            }
        }
    }
}
