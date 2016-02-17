using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Crs.Runtime
{
    public interface IExecutionStrategy
    {
        Task<CommandResponse> ExecuteAsync( CommandContext context );
    }
}
