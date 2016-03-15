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
        /// Dispatch a <see cref="CommandResponse"/> to the identified callback identifier.
        /// </summary>
        /// <param name="callbackId"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        Task DispatchAsync( string callbackId, CommandResponse response, CancellationToken cancellationToken = default( CancellationToken ) );
    }

}
