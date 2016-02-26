
using System;
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
            if( context.ExecutionContext.IsLongRunning && typeof( RequestCommand ).IsAssignableFrom( context.Description.CommandType ) )
                context.ExecutionContext.Monitor.Warn().Send( "The command {0} is describes as a long running command, but inherits from RequestCommand.", context.Description.Name );

            if( context.ExecutionContext.IsLongRunning && !typeof( RequestCommand ).IsAssignableFrom( context.Description.CommandType ) )
                return _async();

            return _inprocess();
        }
    }
}
