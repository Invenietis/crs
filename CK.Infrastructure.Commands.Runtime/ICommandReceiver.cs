using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    /// <summary>
    /// Defines the contract a command receiver should implement
    /// </summary>
    public interface ICommandReceiver
    {
        /// <summary>
        /// Process a <see cref="ICommandRequest"/> and returns a <see cref="ICommandResponse"/>
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<ICommandResponse> ProcessCommandAsync( ICommandRequest command, CancellationToken cancellationToken = default( CancellationToken ) );
    }

}
