
using System;

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
            if( context.ExecutionContext.IsLongRunning ) return _async();
            return _inprocess();
        }
    }
}
