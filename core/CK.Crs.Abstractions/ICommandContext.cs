using System.Threading;
using System.Threading.Tasks;
using CK.Core;
using System;

namespace CK.Crs
{
    public interface ICommandContext
    {
        /// <summary>
        /// Command identifier
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the <see cref="IActivityMonitor"/> that goes with the command throughout its lifetime.
        /// </summary>
        IActivityMonitor Monitor { get; }
        
        /// <summary>
        /// Notify when an underlying component has cancel the execution of this command...
        /// </summary>
        CancellationToken Aborted { get; }

        /// <summary>
        /// Gets the identifier of the caller of this command context.
        /// </summary>
        string CallerId { get; }
    }
}