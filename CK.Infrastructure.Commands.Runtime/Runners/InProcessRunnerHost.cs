using System;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public class InProcessRunnerHost : ICommandRunnerHost
    {
        public Task HostJob( ICommandRunner runner, CommandExecutionContext ctx, CancellationToken cancellation = default( CancellationToken ) )
        {
            return runner.RunAsync( ctx );
        }
    }
}
