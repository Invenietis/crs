using CK.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Crs.Runtime
{
    /// <summary>
    /// </summary>
    public interface ICommandResponseDispatcher
    {
        /// <summary>
        /// Dispatch a <see cref="CommandResponseBuilder"/> to the identified callback identifier.
        /// </summary>
        /// <param name="callbackId"></param>
        /// <param name="response"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DispatchAsync( IActivityMonitor monitor, string callbackId, CommandResponseBuilder response, CancellationToken cancellationToken = default( CancellationToken ) );
    }

}
