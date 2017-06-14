using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CK.Crs.Runtime;

namespace CK.Crs
{
    /// <summary>
    /// Defines the contract a crs handler should implement
    /// </summary>
    public interface ICrsHandler
    {
        /// <summary>
        /// Process a <see cref="CommandRequest"/> and potentially writes to the given <see cref="Stream"/> a response.
        /// </summary>
        /// <param name="command"><see cref="CommandRequest"/></param>
        /// <param name="response">The output stream to output a <see cref="CommandResponseBuilder"/></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns true if the command has been processed (a <see cref="CommandResponse"/> has been created.).</returns>
        Task ProcessCommandAsync( CommandRequest command, CommandResponseBuilder response, CancellationToken cancellationToken = default( CancellationToken ) );
    }

}
