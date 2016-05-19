using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;
using CK.Crs.Runtime;

namespace CK.Crs.Runtime.Execution
{
    class TaskBasedCommandExecutor : AbstractCommandExecutor
    {
        public static readonly string Trait = "Async";

        public TaskBasedCommandExecutor( ICommandHandlerFactory factory ) : base( factory )
        {
        }

        protected override bool CanExecute( IPipeline pipeline, CommandDescription commandDescription )
        {
            var executorTrait = pipeline.Configuration.TraitContext.FindOnlyExisting( TaskBasedCommandExecutor.Trait );

            // Traits can be: Asynchronous|Anonymous|Premium
            CKTrait trait = pipeline.Configuration.TraitContext.FindOrCreate( commandDescription.Traits );
            return trait.IsSupersetOf( executorTrait );
        }

        protected override Task<CommandResponse> ExecuteAsync( IPipeline pipeline, CommandContext context )
        {
            var token = context.ExecutionContext.Monitor.DependentActivity().CreateTokenWithTopic( GetType().Name );

            // This implementation does not guarantee that the command will be correctly handled...
            // We need some retry mechanism and a pending command persistence mechanism to be resilient.
            var t = new Task( async () =>
            {
                // We override the IActivityMonitor with a dependant one to be thread safe !
                using( var dependentMonitor = token.CreateDependentMonitor() )
                {
                    context.ExecutionContext.Monitor = dependentMonitor;

                    await CommandRunner.ExecuteAsync( context, Factory );

                    var response = new CommandResultResponse( context.Result, context.ExecutionContext.Action );
                    if( pipeline.Configuration.ExternalComponents.ResponseDispatcher == null )
                        dependentMonitor.Warn().Send("No response dispatcher were available...");
                    else
                        await pipeline.Configuration.ExternalComponents.ResponseDispatcher.DispatchAsync( context.ExecutionContext.Action.CallbackId, response );
                }
            } );

            var deferredResponse = new CommandDeferredResponse( context.ExecutionContext.Action );
            t.Start( TaskScheduler.Current );
            return Task.FromResult<CommandResponse>( deferredResponse );
        }
    }
}
