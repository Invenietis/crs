
using System;
using System.Reflection;
using CK.Core;

namespace CK.Crs.Runtime
{
    public class BasicExecutionStrategySelector : IExecutionStrategySelector
    {
        readonly InProcessExecutionStrategy _inprocess;
        readonly AsyncExecutionStrategy _async;

        public BasicExecutionStrategySelector( ICommandHandlerFactory commandHandlerFactory, ICommandDecoratorFactory c )
        {
            _inprocess = new InProcessExecutionStrategy(  );
            _async = new AsyncExecutionStrategy( , commandHandlerFactory.CreateResponseDispatcher );
        }

        public virtual IExecutionStrategy SelectExecutionStrategy( CommandContext context )
        {
            var isLongRunning =  context.ExecutionContext.Action.Description.Descriptor.IsLongRunning;
            var commandType = context.ExecutionContext.Action.Description.Descriptor.CommandType;

            if( isLongRunning && typeof( RequestCommand ).IsAssignableFrom( commandType ) )
            {
                string msg =  $"The command {context.ExecutionContext.Action.Description.Descriptor.Name} is described as a long running command, but inherits from RequestCommand.";
                context.ExecutionContext.Monitor.Warn().Send( msg );
            }

            if( isLongRunning && !typeof( RequestCommand ).IsAssignableFrom( commandType ) ) return _async;

            return _inprocess;
        }
    }
}
