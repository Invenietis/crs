
using System;

namespace CK.Crs
{
    public class ExecutionStrategySelector : IExecutionStrategySelector
    {
        readonly Func<InProcessExecutionStrategy> _inprocess;
        readonly Func<AsyncExecutionStrategy> _async;

        public ExecutionStrategySelector( ICommandReceiverFactories factory )
        {
            var runner = new CommandRunner( factory);
            _inprocess = () => new InProcessExecutionStrategy( runner );
            _async = () => new AsyncExecutionStrategy( runner, factory.CreateResponseDispatcher );
        }

        public virtual IExecutionStrategy SelectExecutionStrategy( CommandContext context )
        {
            if( context.Command.IsLongRunning ) return _async();
            return _inprocess();
        }
    }
}
