using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{
    public interface ICommandExecutionContext
    {
        /// <summary>
        /// Gets the <see cref="IActivityMonitor"/> that goes with the command throughout its lifetime.
        /// </summary>
        IActivityMonitor Monitor { get; }
        
        /// <summary>
        /// Notify when an underlying component has cancel the execution of this command...
        /// </summary>
        CancellationToken CommandAborted { get; }

        string CallbackId { get; }
    }
}