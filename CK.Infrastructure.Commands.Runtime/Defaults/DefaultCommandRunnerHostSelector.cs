namespace CK.Infrastructure.Commands
{
    public class DefaultCommandRunnerHostSelector : ICommandRunnerHostSelector
    {
        AsyncCommandRunner _asyncCommandRunnerHost;
        CommandRunnerLocalHost _localHost;

        public DefaultCommandRunnerHostSelector( ICommandResponseDispatcher dispatcher )
        {
            _localHost = new CommandRunnerLocalHost();
            _asyncCommandRunnerHost = new AsyncCommandRunner( dispatcher );
        }

        public ICommandRunnerHost SelectHost( CommandContext runtimeContext )
        {
            if( runtimeContext.IsLongRunning ) return _asyncCommandRunnerHost;
            return _localHost;
        }
    }
}
