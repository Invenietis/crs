
using System;
using System.Reflection;
using CK.Core;

namespace CK.Crs.Runtime
{
    public class BasicExecutionStrategySelector : IExecutionStrategySelector
    {
        readonly Func<InProcessExecutionStrategy> _inprocess;
        readonly Func<AsyncExecutionStrategy> _async;

        public BasicExecutionStrategySelector( IFactories factory )
        {
            var runner = new CommandRunner( factory);
            _inprocess = () => new InProcessExecutionStrategy( runner );
            _async = () => new AsyncExecutionStrategy( runner, factory.CreateResponseDispatcher );
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

            if( isLongRunning && !typeof( RequestCommand ).IsAssignableFrom( commandType ) ) return _async();

            return _inprocess();
        }
    }
}
