using CK.Core;
using System;
using System.Threading;

namespace CK.Crs.Hosting
{
    public interface ICommandHost
    {
        void Start( IActivityMonitor monitor, IServiceProvider services, CancellationToken cancellationToken = default );
        void Stop( IActivityMonitor monitor, IServiceProvider services, CancellationToken cancellationToken = default );
    }
}
