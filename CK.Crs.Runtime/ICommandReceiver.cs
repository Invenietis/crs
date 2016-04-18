using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CK.Crs.Pipeline;

namespace CK.Crs
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
        Task ProcessCommandAsync( CommandRequest command, Stream output, CancellationToken cancellationToken = default( CancellationToken ) );
    }

}
