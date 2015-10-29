using Microsoft.Extensions.OptionsModel;

namespace CK.Infrastructure.Commands
{
    public class CommandRunnerHostSelector : ICommandRunnerHostSelector
    {
        InProcessRunnerHost _localHost;
        TaskRunnerHost _taskBasedHost;

        public CommandRunnerHostSelector( ICommandResponseDispatcher dispatcher )
        {
            _localHost = new InProcessRunnerHost();
            _taskBasedHost = new TaskRunnerHost( dispatcher );
        }

        public ICommandRunnerHost SelectHost( CommandContext runtimeContext )
        {
            if( runtimeContext.IsLongRunning ) return _taskBasedHost;
            return _localHost;
        }
    }
}
