using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CK.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;

namespace CK.Crs.Tests
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
            get { return PlatformServices.Default.Application; }
        }

        public static IActivityMonitor Monitor( TextWriter output )
        {
            var mon = new ActivityMonitor();
            mon.Output.RegisterClient( new ActivityMonitorTextWriterClient( output.WriteLine ) );
            return mon;
        }

        internal static string GeneratorResultPath( string testName )
        {
            return Path.Combine( ApplicationEnv.ApplicationBasePath, "TestOutputs", $"{testName}_{DateTime.UtcNow.ToString( "HHmmss" )}.log" );
        }
    }
}
