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

        /// <summary>
        /// Implementations should tell if the are capable to execute a command described by its <see cref="CommandDescription"/>.
        /// </summary>
        /// <param name="pipeline">The current execution <see cref="IPipeline"/></param>
        /// <param name="commandDescription">The description of the command to execute.</param>
        /// <returns>True if the command executor can execute the command, false otherwise.</returns>
        protected abstract bool CanExecute( IPipeline pipeline, CommandDescription commandDescription );

        public override bool ShouldInvoke( IPipeline pipeline )
        {
            return pipeline.Response == null && pipeline.Action.Command != null && CanExecute( pipeline, pipeline.Action.Description.Descriptor );
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
