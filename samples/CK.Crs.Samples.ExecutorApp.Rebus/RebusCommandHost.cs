using CK.Core;
using CK.Crs.Samples.Handlers;
using CK.Crs.Samples.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using System;
using System.IO;
using Rebus.SqlServer;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Rebus.Transport;
using Rebus.Timeouts;
using Rebus.Pipeline;
using Rebus.Pipeline.Receive;
using Rebus.Logging;
using Rebus.SqlServer.Transport;
using Rebus.Injection;
using Rebus.Threading;
using System.Collections.Generic;
using Rebus.Messages;

namespace CK.Crs.Samples.ExecutorApp.Rebus
{
    class RebusCommandHost : ICommandHost
    {
        public void Init( IActivityMonitor monitor, IServiceCollection services )
        {
            var builder = new ConfigurationBuilder()
                        .SetBasePath( Directory.GetCurrentDirectory() )
                        .AddJsonFile( "appsettings.json", optional: true, reloadOnChange: true );

            var configuration = builder.Build();

            var conString = configuration.GetConnectionString( "Messaging" );

            services
                .AddCrsCore(
                    c => c.Commands( registry => registry
                        .Register<RemotelyQueuedCommand>().HandledBy<RemoteHandler>()
                        .Register<RemotelyQueuedCommand.Result>().IsRebus() ) )
                .AddRebus(
                    c => c.Transport( t => t.UseSqlServer( conString, "tMessages", "commands" ) ),
                    c => c( "commands_result" )( m => m.HasRebusTag() ) );

        }

        public void Start( IActivityMonitor monitor, IServiceProvider services )
        {
            var receiver = services.GetRequiredService<ICommandReceiver>();
            monitor.Trace( $"Rebus receiver {receiver.Name} available" );
        }

        public void Stop( IActivityMonitor monitor, IServiceProvider services )
        {
        }
    }
}
