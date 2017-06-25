using System;
using System.Threading;

namespace CK.Crs.Scalability
{
    public interface ICrsHost
    {
        /// <summary>
        /// Wait for messages.
        /// </summary>
        /// <param name="token"></param>
        void WaitForMessages( CancellationToken token = default( CancellationToken ));
    }
}