using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CK.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Xunit.Abstractions;

namespace CK.Infrastructure.Commands.Tests
{
    public class TestHelper
    {
        public static IServiceProvider CreateServiceProvider( Action<IServiceCollection> setup )
        {
            var services = new ServiceCollection();
            if( setup != null )
            {
                setup( services );
            }

            return services.BuildServiceProvider();
        }
        static IApplicationEnvironment ApplicationEnv
        {
            get { return GetRequiredService<IApplicationEnvironment>(); }
        }

        public static IActivityMonitor Monitor( ITestOutputHelper output )
        {
            var mon = new ActivityMonitor();
            mon.Output.RegisterClient( new ActivityMonitorTextWriterClient( output.WriteLine ) );
            return mon;
        }

        static private T GetRequiredService<T>()
        {
            return CallContextServiceLocator.Locator.ServiceProvider.GetRequiredService<T>();
        }

        internal static string GeneratorResultPath( string testName )
        {
            return Path.Combine( ApplicationEnv.ApplicationBasePath, "TestOutputs", $"{testName}_{DateTime.UtcNow.ToString( "HHmmss" )}.log" );
        }
    }
}
