using CK.Core;
using CK.Crs.Runtime;
using Paramore.Brighter;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Crs.Runtime.Execution
{
    class BrighterMediator : AbstractCommandExecutor
    {
        MethodInfo _sender;
        IAmACommandProcessor _commandProcessor;
        CKTrait _componentTrait;
        ICommandRunningStore _commandRunningStore;

        public BrighterMediator(IAmACommandProcessor commandProcessor, ICommandRunningStore store, ICommandRegistry registry, CKTrait componentTrait ) : base(registry)
        {
            _commandProcessor = commandProcessor;
            _commandRunningStore = store;
            _componentTrait = componentTrait;
            _sender = _commandProcessor.GetType().GetTypeInfo().GetMethod( "SendAsync" ).GetGenericMethodDefinition();
        }

        protected override bool CanExecute(IPipeline pipeline, CommandDescription commandDescription)
        {
            return _componentTrait.Overlaps( commandDescription.Traits );
        }

        protected async override Task<CommandResponse> ExecuteAsync(IPipeline pipeline, CommandContext context)
        {
            if (String.IsNullOrEmpty(context.ExecutionContext.Action.CallbackId))
                throw new InvalidOperationException("You must supply a CallbackId in order to be notified of command responses...");

            // This implementation does not guarantee that the command will be correctly handled...
            // We need some retry mechanism and a pending command persistence mechanism to be resilient.

            await _commandRunningStore.AddCommandAsync(context.ExecutionContext.Action.CallbackId, context.ExecutionContext.Action.CommandId);

            var token = context.ExecutionContext.Monitor.DependentActivity().CreateTokenWithTopic(GetType().Name);
            await (Task)_sender.MakeGenericMethod(pipeline.Action.Description.CommandType).Invoke(_commandProcessor, new[] { pipeline.Action.Command });

            var deferredResponse = new CommandDeferredResponse(context.ExecutionContext.Action);
            return deferredResponse;
        }
    }
}
