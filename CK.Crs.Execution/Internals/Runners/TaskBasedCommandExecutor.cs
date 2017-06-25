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

        readonly ICommandRunningStore _commandRunningStore;
        readonly IExecutionFactory _factory;
        readonly CKTrait _componentTrait;
        public TaskBasedCommandExecutor( ICommandRunningStore commandRunningStore, IExecutionFactory factory, ICommandRegistry registry, CKTrait trait ) : base( registry )
        {
            _commandRunningStore = commandRunningStore;
            _factory = factory;
            _componentTrait = trait;
        }

        protected override bool CanExecute( IPipeline pipeline, CommandDescription commandDescription )
        {
            // Traits can be: Asynchronous|Anonymous|Premium
            return _componentTrait.Overlaps( commandDescription.Traits );
        }

        protected override async Task<CommandResponse> ExecuteAsync( IPipeline pipeline, CommandContext context )
        {
            if( String.IsNullOrEmpty( context.ExecutionContext.Action.CallbackId ) )
                throw new InvalidOperationException( "You must supply a CallbackId in order to be notified of command responses..." );

            // This implementation does not guarantee that the command will be correctly handled...
            // We need some retry mechanism and a pending command persistence mechanism to be resilient.

            await _commandRunningStore.AddCommandAsync( context.ExecutionContext.Action.CallbackId, context.ExecutionContext.Action.CommandId );

            var token = context.ExecutionContext.Monitor.DependentActivity().CreateTokenWithTopic( GetType().Name );
            await Task.Run( async () =>
            {
                // We override the IActivityMonitor with a dependant one to be thread safe !
                var dependentMonitor = new ActivityMonitor();
                using( dependentMonitor.StartDependentActivity( token ) )
                {
                    context.ExecutionContext.Monitor = dependentMonitor;

                    await Engine.RunAsync( context, _factory );

                    var dispatcher = pipeline.Configuration.ExternalComponents.ResponseDispatcher;
                    if (dispatcher == null) dependentMonitor.Warn().Send("No response dispatcher were available...");
                    else
                    { 
                        var response = CreateFromContext( context );
                        pipeline.Response.Set( response );
                        await dispatcher.DispatchAsync( 
                            context.ExecutionContext.Monitor, 
                            context.ExecutionContext.Action.CallbackId, 
                            pipeline.Response );
                    }
                }
            } );


            var deferredResponse = new CommandDeferredResponse( context.ExecutionContext.Action );
            return deferredResponse;
        }
    }
}
