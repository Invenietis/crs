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
        /// <param name="output">The output stream to output a <see cref="CommandResponse"/></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns true if the command has been processed (a <see cref="CommandResponse"/> has been created.).</returns>
        Task<bool> ProcessCommandAsync( CommandRequest command, Stream output, CancellationToken cancellationToken = default( CancellationToken ) );
    }

}
