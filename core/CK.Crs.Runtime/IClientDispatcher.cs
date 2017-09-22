using System;
namespace CK.Crs
{
    public interface IClientDispatcher : IDisposable
    {
        /// <summary>
        /// Sends a message to the caller identified in the <see cref="ICommandContext"/>.
        /// </summary>
        /// <param name="message">The message to send.</param>
        void Send<T>( string callerId, Response<T> response );

        /// <summary>
        /// Broadcast the message to all connected clients.
        /// </summary>
        /// <param name="message">The message to broadcast.</param>
        void Broadcast<T>( Response<T> response );
    }
}
