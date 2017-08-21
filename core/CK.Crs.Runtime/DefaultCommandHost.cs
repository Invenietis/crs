using CK.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

namespace CK.Crs
{
    public class DefaultCommandHost
    { 
        public static void Run<T>( CancellationToken token ) where T : ICommandHost
        {
            ServiceCollection collection = new ServiceCollection();
            IActivityMonitor monitor = new ActivityMonitor();

            T s = Activator.CreateInstance<T>();
            s.Init( monitor, collection );
            using( var services = collection.BuildServiceProvider() )
            {
                try
                {
                    s.Start( monitor, services );
                    token.WaitHandle.WaitOne();
                }
                catch( Exception ex )
                {
                    monitor.Error( ex );
                }
                finally
                {
                    s.Stop( monitor, services );
                }
            }
        }
    }
}
