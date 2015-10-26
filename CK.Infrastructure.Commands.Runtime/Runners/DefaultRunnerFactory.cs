using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public class DefaultRunnerFactory : ICommandRunnerFactory
    {
        private CommandRunner _syncRunner;

        public DefaultRunnerFactory( ICommandHandlerFactory handlerFactory )
        {
            _syncRunner = new CommandRunner( handlerFactory );
        }

        public ICommandRunner CreateRunner( CommandContext runtimeContext )
        {
            return _syncRunner;
        }
    }
}
