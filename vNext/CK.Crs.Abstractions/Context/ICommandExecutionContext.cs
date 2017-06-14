using System;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{
    public interface ICommandExecutionContext
    {
        /// <summary>
        /// 
        /// </summary>
        CommandAction Action { get; }

        /// <summary>
        /// Gets the <see cref="IActivityMonitor"/> that goes with the command throughout its lifetime.
        /// </summary>
        IActivityMonitor Monitor { get; set; }
        
        /// <summary>
        /// Notify when an underlying component has cancel the execution of this command...
        /// </summary>
        CancellationToken CommandAborted { get; set; }

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