using System;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Infrastructure.Commands
{
    public class InProcessRunnerHost : ICommandRunnerHost
    {
        public async Task HostJob( IActivityMonitor monitor, ICommandRunner runner, CommandExecutionContext ctx, CancellationToken cancellation = default( CancellationToken ) )
        {
            try
            {
                await runner.RunAsync( ctx );
                monitor.Trace().Send( "Done." );
            }
            catch( Exception ex )
            {
                monitor.Error().Send( ex );
                throw;
            }
        }
    }
}
