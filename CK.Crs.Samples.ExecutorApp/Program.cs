using CK.Core;
using CK.Crs.Runtime;
using CK.Crs.Runtime.Execution;
using CK.Crs.Scalability;
using CK.Crs.Scalability.Redis;
using CK.Crs.Scalability.Tcp;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs.Samples.ExecutorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var crs = new CrsHostBuilder()
                .UserServices( new ServiceCollection() )
                .UseStartup<Startup>()
                .UseActivityMonitor( new ActivityMonitor() )
                .Build();

            crs.WaitForMessages();
        }
    }
}