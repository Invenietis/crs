using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;
using CK.Crs.Pipeline;

namespace CK.Crs.Runtime.Execution
{
    public class TaskBasedCommandExecutor : AbstractCommandExecutor
    {
        public TaskBasedCommandExecutor( ICommandExecutionFactories factories ) : base( factories )
        {
        }

        protected override bool CanExecute( CommandDescription commandDescription )
        {
            return commandDescription.IsLongRunning;
        }

        protected override Task<CommandResponse> ExecuteAsync( CommandContext context )
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

                    await CommandRunner.ExecuteAsync( context, Factories );

                    var response = new CommandResultResponse( context.Result, context.ExecutionContext.Action );
                    var responseDispatcher = Factories.CreateResponseDispatcher();
                    if( responseDispatcher == null )
                        dependentMonitor.Warn().Send("No response dispatcher were available...");
                    else
                        await responseDispatcher.DispatchAsync( context.ExecutionContext.Action.CallbackId, response );
                }
            } );

            var deferredResponse = new CommandDeferredResponse( context.ExecutionContext.Action );
            t.Start( TaskScheduler.Current );
            return Task.FromResult<CommandResponse>( deferredResponse );
        }
    }
}
