using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public interface ICommandRunner
    {
        Task<ICommandResponse> RunAsync( CommandProcessingContext ctx, CancellationToken cancellationToken = default( CancellationToken ) );
    }
}
