using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public interface ICommandRunnerHost
    {
        Task HostJob( ICommandRunner runner, CommandExecutionContext ctx, CancellationToken cancellation = default( CancellationToken ) );
    }
}
