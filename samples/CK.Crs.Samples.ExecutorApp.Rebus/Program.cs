using CK.Core;
using CK.Crs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using CK.Crs.Samples.Messages;
using CK.Crs.Samples.Handlers;
using Rebus.Config;

namespace CK.Crs.Samples.ExecutorApp.Rebus
{
    class Program
    {

        static async Task Main( string[] args )
        {
            var host = new HostBuilder()
                .ConfigureHostConfiguration( configHost =>
                {
                    configHost.AddJsonFile( "appsettings.json", optional: true, reloadOnChange: true );
                } )
                .ConfigureServices( ( ctx, services ) =>
                {
                    var conString = ctx.Configuration.GetConnectionString( "Messaging" );

                    services
                        .AddHostedService<RebusCommandHost>()
                        .AddCrsCore( registry =>
                        {
                            registry.Register<RemotelyQueuedCommand>().HandledBy<RemoteHandler>();
                            registry.Register<RemotelyQueuedCommand.Result>().IsRebus();
                        } )
                        .AddRebus(
                            c => c.Transport( t => t.UseSqlServer( conString, "commands" ) ),
                            c => c( "commands_result" )( m => m.HasRebusTag() ) );
                } );

            await host.RunConsoleAsync();
        }
    }
}
