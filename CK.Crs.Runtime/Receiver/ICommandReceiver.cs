using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime
{
    /// <summary>
    /// Defines the contract a command receiver should implement
    /// </summary>
    public interface ICommandReceiver
    {
        /// <summary>
        /// Process a <see cref="CommandRequest"/> and returns a <see cref="CommandResponse"/>
        /// </summary>
        /// <param name="command"><see cref="CommandRequest"/></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<CommandResponse> ProcessCommandAsync(  CommandRequest command, CancellationToken cancellationToken = default( CancellationToken ) );
    }

}
