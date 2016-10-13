using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{
    public interface IExternalEventPublisher
    {
        /// <summary>
        /// Pushs an events to be published.
        /// </summary>
        /// <typeparam name="T">The event type. Implementations should serialize this type.</typeparam>
        /// <param name="monitor">The <see cref="IActivityMonitor"/> that monitors the operation.</param>
        /// <param name="event">The object to serialize.</param>
        void Push<T>( IActivityMonitor monitor, T @event );
        /// <summary>
        /// Force the push of an event to the external world possibly bypassing any current transaction or compensation system.
        /// </summary>
        /// <typeparam name="T">The event type. Implementations should serialize this type.</typeparam>
        /// <param name="monitor">The <see cref="IActivityMonitor"/> that monitors the operation.</param>
        /// <param name="event">The object to serialize.</param>
        void ForcePush<T>( IActivityMonitor monitor, T @event );
    }
}
