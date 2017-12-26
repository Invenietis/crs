using CK.Core;
using CK.Crs.Hosting;
using CK.Monitoring;
using System;
using System.Threading;

namespace CK.Crs.Samples.ExecutorApp.Rebus
{
    class Program
    {

        static void Main( string[] args )
        {
            using( CancellationTokenSource cancellationTokenSource = new CancellationTokenSource() )
            {
                Console.CancelKeyPress += ( sender, e ) => cancellationTokenSource.Cancel();

                using( GrandOutput.EnsureActiveDefault( new GrandOutputConfiguration
                {
                    Handlers = { new CK.Monitoring.Handlers.TextFileConfiguration { Path = "logs/", MaxCountPerFile = 100 } }
                } ) )
                {
                    ActivityMonitor.AutoConfiguration = ( m ) => m.Output.RegisterClient( new ActivityMonitorConsoleClient() );
                  
                    DefaultCommandHost.Run<RebusCommandHost>( cancellationTokenSource.Token );
                }
            }
        }
    }
}
