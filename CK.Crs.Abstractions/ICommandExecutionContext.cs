using System;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{
    public interface ICommandExecutionContext
    {
        /// <summary>
        /// Gets the unique command identifier.
        /// </summary>
        Guid CommandId { get; }
        /// <summary>
        /// Gets a callback identifier used to identify the originator of the request.
        /// </summary>
        string CallbackId { get; }
        /// <summary>
        /// Gets the <see cref="IActivityMonitor"/> that goes with the command throughout its lifetime.
        /// </summary>
        IActivityMonitor Monitor { get; }
        /// <summary>
        /// Notify when an underlying component has cancel the execution of this command...
        /// </summary>
        CancellationToken CommandAborted { get; }
        /// <summary>
        /// Gets the <see cref="IExternalEventPublisher"/> where command events can be published to the world!
        /// </summary>
        IExternalEventPublisher ExternalEvents { get; }

        /// <summary>
        /// Gets the <see cref="ICommandScheduler"/> for this execution context.
        /// </summary>
        ICommandScheduler Scheduler { get; }
    }
}