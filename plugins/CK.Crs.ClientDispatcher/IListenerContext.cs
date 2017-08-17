using CK.Core;
using System.Threading;

namespace CK.Crs
{
    public interface IListenerContext
    {
        string ClientId { get; }

        IActivityMonitor Monitor { get; }

        /// <summary>
        /// Notify when an underlying component has cancel the execution of this command...
        /// </summary>
        CancellationToken Aborted { get; }
    }
}
