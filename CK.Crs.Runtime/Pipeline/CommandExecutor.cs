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

        public CommandExecutor( IPipeline pipeline, IFactories factories )
            : this( pipeline, factories, new BasicExecutionStrategySelector( factories ) )
        {
        }

        public CommandExecutor( IPipeline pipeline, IFactories factories, IExecutionStrategySelector strategy ) : base( pipeline )
        {
            _factories = factories;
            _executorSelector = strategy;
        }

        public override bool ShouldInvoke
        {
            get { return Pipeline.Response == null && Pipeline.Action.Command != null; }
        }

        public override async Task Invoke( CancellationToken token )
        {
            CommandExecutionContext executionContext = new CommandExecutionContext(
                    Pipeline.Action,
                    Monitor,
                    token,
                    _factories.CreateExternalEventPublisher,
                    _factories.CreateCommandScheduler);

            var context = new CommandContext( executionContext );

            using( Pipeline.Monitor.OpenTrace().Send( "Running command..." ) )
            {
                var strategy = _executorSelector.SelectExecutionStrategy( context );
                if( strategy == null )
                {
                    string msg = "The Selector should returns a valid, non null executor... otherwise, commands will produce nothing!";
                    throw new ArgumentNullException( msg );
                }

                Pipeline.Monitor.Trace().Send( $"[ExecutionStrategy={strategy.GetType().Name}]" );

                if( Pipeline.Events.CommandExecuting != null )
                    await Pipeline.Events.CommandExecuting?.Invoke( context );

                Pipeline.Response = await strategy.ExecuteAsync( context );

                // REVIEW: the context should be different 
                // because at this stage we do not want to offe the possibility to sets the result
                //await Pipeline.Events.CommandExecuted?.Invoke( context );
            }
        }
    }
}
