using CK.Core;
using CK.Crs.Samples.Handlers;
using CK.Crs.Samples.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using System;
using System.IO;
using CK.Crs.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CK.Crs.Samples.ExecutorApp.Rebus
{
    class RebusCommandHost : BackgroundService
    {
        private readonly ICommandReceiver _commandReceiver;
        private readonly ILogger<RebusCommandHost> _logger;

        public RebusCommandHost( ICommandReceiver commandReceiver, ILogger<RebusCommandHost> logger )
        {
            _commandReceiver = commandReceiver;
            _logger = logger;
        }

        protected override Task ExecuteAsync( CancellationToken stoppingToken )
        {
            _logger.LogTrace( $"Rebus receiver {_commandReceiver.Name} available" );

            return Task.CompletedTask;
        }
    }
}
