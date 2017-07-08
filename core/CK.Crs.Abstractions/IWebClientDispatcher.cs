using CK.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs
{
    public interface IWebClientDispatcher : IDisposable
    {
        /// <summary>
        /// Sends a message to the caller identified in the <see cref="ICommandContext"/>.
        /// </summary>
        /// <param name="message">The message to send.</param>
        void Send<T>( T message, ICommandContext context );

        /// <summary>
        /// Broadcast the message to all connected clients.
        /// </summary>
        /// <param name="message">The message to broadcast.</param>
        void Broadcast<T>( T message, ICommandContext context );
    }
}
