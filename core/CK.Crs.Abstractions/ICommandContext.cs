using System.Threading;
using System.Threading.Tasks;
using CK.Core;
using System;

namespace CK.Crs
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICommandContext : IFeatureContainer
    {
        /// <summary>
        /// Command identifier
        /// </summary>
        string CommandId { get; }

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
        CallerId CallerId { get; }

        /// <summary>
        /// Gets the related <see cref="CommandModel"/> for the given executing command.
        /// </summary>
        ICommandModel Model { get; }


        IEndpointModel EndpointModel { get; }
    }
}
