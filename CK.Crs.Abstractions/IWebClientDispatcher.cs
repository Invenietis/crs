using CK.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs
{
    public interface IWebClientDispatcher : IDisposable
    {
        /// <summary>
        /// Sends the given message to the given client.
        /// </summary>
        /// <param name="message"></param>
        void Send<T>( T message, ICommandContext context );

        /// <summary>
        /// Broadcast the message
        /// </summary>
        /// <param name="message"></param>
        void Broadcast<T>( T message, ICommandContext context );

    }
}
