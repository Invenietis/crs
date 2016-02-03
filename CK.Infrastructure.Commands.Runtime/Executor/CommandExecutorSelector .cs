using Microsoft.Extensions.OptionsModel;

namespace CK.Infrastructure.Commands
{
    public class CommandExecutorSelector : ICommandExecutorSelector
    {
        readonly InProcessCommandExecutor _localHost;
        readonly OutProcessCommandExecutor _taskBasedHost;

        public CommandExecutorSelector( ICommandResponseDispatcher dispatcher, ICommandReceiverFactories factory )
        {
            _localHost = new InProcessCommandExecutor( factory );
            _taskBasedHost = new OutProcessCommandExecutor( dispatcher, factory );
        }

        public virtual ICommandExecutor SelectExecutor( CommandContext runtimeContext )
        {
            if( runtimeContext.IsLongRunning ) return _taskBasedHost;
            return _localHost;
        }
    }
}
