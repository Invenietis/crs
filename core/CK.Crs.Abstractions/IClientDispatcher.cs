using CK.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs
{
    public interface IClientDispatcher : IDisposable
    {
        /// <summary>
        /// Sends a message to the caller identified in the <see cref="ICommandContext"/>.
        /// </summary>
        /// <param name="message">The message to send.</param>
        Task Send<T>( string eventName, T message, ICommandContext context );

        /// <summary>
        /// Broadcast the message to all connected clients.
        /// </summary>
        /// <param name="message">The message to broadcast.</param>
        Task Broadcast<T>( string eventName, T message, ICommandContext context );
    }
}
