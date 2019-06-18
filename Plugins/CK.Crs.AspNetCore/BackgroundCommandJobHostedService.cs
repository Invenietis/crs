using CK.Core;
using CK.Crs.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Crs.AspNetCore
{
    public class BackgroundCommandJobHostedService : IHostedService
    {
        private readonly ICommandHost _commandHost;
        private readonly IServiceProvider _serviceProvider;

        public BackgroundCommandJobHostedService( ICommandHost commandHost, IServiceProvider serviceProvider )
        {
            _commandHost = commandHost;
            _serviceProvider = serviceProvider;
        }
        public Task StartAsync( CancellationToken cancellationToken )
        {
            _commandHost.Start( new ActivityMonitor(), _serviceProvider );
            return Task.CompletedTask;
        }

        public Task StopAsync( CancellationToken cancellationToken )
        {
            _commandHost.Stop( new ActivityMonitor(), _serviceProvider );
            return Task.CompletedTask;
        }
    }

}
