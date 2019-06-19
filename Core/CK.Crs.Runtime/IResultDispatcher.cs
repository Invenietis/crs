using System;
using System.Threading.Tasks;

namespace CK.Crs
{
    public interface IResultDispatcher : IDisposable
    {
        /// <summary>
        /// Sends a message to the caller identified in the <see cref="ICommandContext"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="response">The message to send.</param>
        Task Send<T>( ICommandContext context, Response<T> response );

        /// <summary>
        /// Broadcast the message to all connected clients.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="response">The message to broadcast.</param>
        Task Broadcast<T>( ICommandContext context, Response<T> response );
    }
}
