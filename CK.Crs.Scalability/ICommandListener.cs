using System;
using System.Threading;

namespace CK.Crs.Scalability
{
    /// <summary>
    /// Defines a command listener, whiche receives jobs and push them to the shared <see cref="ICommandWorkerQueue"/>.
    /// </summary>
    public interface ICommandListener
    {
        /// <summary>
        /// Starts the receive process, with the ability to cancel it using the provided <see cref="CancellationToken"/>.
        /// </summary>
        /// <param name="workerQueue">The queue to push jobs</param>
        /// <param name="token">The cancellation token</param>
        void Receive( ICommandWorkerQueue workerQueue, CancellationToken token );
    }


}
