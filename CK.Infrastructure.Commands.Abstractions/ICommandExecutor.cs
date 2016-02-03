using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public interface ICommandExecutor
    {
        /// <summary>
        /// Executes the command! Implementations must honor the <see cref="CancellationToken"/>...
        /// </summary>
        /// <param name="ctx">An execution context for this command.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns><see cref="Task"/></returns>
        Task ExecuteAsync( CommandExecutionContext ctx, CancellationToken cancellationToken = default( CancellationToken ) );
    }
}
