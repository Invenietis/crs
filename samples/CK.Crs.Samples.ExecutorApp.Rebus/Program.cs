using CK.Core;
using CK.Monitoring;
using Microsoft.Extensions.DependencyInjection;
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

                    RunService<RebusCommandService>( cancellationTokenSource.Token );
                    //RunService<RebusEventService>( cancellationTokenSource.Token );
                }
            }
        }

        private static void RunService<T>( CancellationToken token ) where T : IRebusService
        {
            ServiceCollection collection = new ServiceCollection();
            T s = Activator.CreateInstance<T>();

            using( var services = collection.BuildServiceProvider() )
            {
                try
                {
                    s.Init( collection );
                    s.Start( services );
                    token.WaitHandle.WaitOne();
                }
                finally
                {
                    s.Stop( services );
                }
            }
        }
    }
}
