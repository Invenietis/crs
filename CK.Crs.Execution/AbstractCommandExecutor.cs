using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;
using CK.Crs.Runtime;

namespace CK.Crs.Runtime.Execution
{

    public abstract class AbstractCommandExecutor : PipelineComponent
    {
        public AbstractCommandExecutor( ICommandExecutionFactories factories )
        {
            Factories = factories;
        }

        protected ICommandExecutionFactories Factories { get; }

        protected abstract Task<CommandResponse> ExecuteAsync( CommandContext context );

        protected abstract bool CanExecute( CommandDescription commandDescription );

        public override bool ShouldInvoke( IPipeline pipeline )
        {
            return pipeline.Response == null && pipeline.Action.Command != null && CanExecute( pipeline.Action.Description.Descriptor );
        }

        public override async Task Invoke( IPipeline pipeline, CancellationToken token = default( CancellationToken ) )
        {
            using( pipeline.Monitor.OpenTrace().Send( "Preparing asynchronous command execution..." ) )
            {
                var executionContext = new CommandExecutionContext( pipeline.Action, pipeline.Monitor, token, Factories );
                var context = new CommandContext( executionContext );

                if( pipeline.Configuration.Events.CommandExecuting != null )
                    await pipeline.Configuration.Events.CommandExecuting?.Invoke( context );

                if( pipeline.Response == null )
                    pipeline.Response = await ExecuteAsync( context );
            }
        }
        protected CommandResponse CreateFromContext( CommandContext context )
        {
            if( context.IsDirty )
            {
                if( context.Exception != null )
                    return new CommandErrorResponse( context.Exception.Message, context.ExecutionContext.Action.CommandId );

                if( context.Result != null )
                {
                    return new CommandResultResponse( context.Result, context.ExecutionContext.Action );
                }
            }

            return new CommandDeferredResponse( context.ExecutionContext.Action );
        }
    }

}
