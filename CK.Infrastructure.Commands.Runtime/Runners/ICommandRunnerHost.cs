using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Infrastructure.Commands
{
    public interface ICommandRunnerHost
    {
        Task HostJob( IActivityMonitor monitor, ICommandRunner runner, CommandExecutionContext ctx, CancellationToken cancellation = default( CancellationToken ) );
    }
}
