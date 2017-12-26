using CK.Core;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CK.Crs.Hosting
{
    public interface ICommandHost
    {
        void Init( IActivityMonitor monitor, IServiceCollection services );
        void Start( IActivityMonitor monitor, IServiceProvider services );
        void Stop( IActivityMonitor monitor, IServiceProvider services );
    }
}
